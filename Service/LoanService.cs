using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api_flms_service.Service
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly LoanSettings _loanSettings;

        public LoanService(AppDbContext context, INotificationService notificationService, IOptions<LoanSettings> loanSettings)
        {
            _context = context;
            _notificationService = notificationService;
            _loanSettings = loanSettings.Value;

            // Validate LoanDeferredTime to ensure it's within a reasonable range
            if (_loanSettings.LoanDeferredTime < 0 || _loanSettings.LoanDeferredTime > 365 * 1000) // Max 1000 years
            {
                throw new ArgumentException("LoanDeferredTime must be between 0 and 365,000 days (1000 years).");
            }
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.BookLoanId == id);
        }

        public async Task<Loan> CreateLoanAsync(Loan loan)
        {
            // Validate LoanDate to ensure it won't cause an overflow when calculating the due date
            if (loan.LoanDate.HasValue && (loan.LoanDate < DateTime.MinValue || loan.LoanDate > DateTime.MaxValue.AddDays(-_loanSettings.LoanDeferredTime)))
            {
                throw new ArgumentException("LoanDate is out of range and would cause an overflow when calculating the due date.");
            }

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book == null || book.AvailableCopies <= 0)
                throw new Exception("Book is not available");

            book.AvailableCopies--;
            _context.Books.Update(book);
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> ReturnLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;
            if (loan.ReturnDate == null && DateTime.Now > loan.LoanDate.Value.AddDays(_loanSettings.LoanDeferredTime)) return false;

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
                _context.Books.Update(book);
            }

            loan.ReturnDate = DateTime.Now;
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();

            var nextReservation = await _context.Loans
                .Where(l => l.BookId == loan.BookId && l.LoanDate == DateTime.MaxValue && l.ReturnDate == null)
                .OrderBy(l => l.BookLoanId)
                .FirstOrDefaultAsync();

            if (nextReservation != null)
            {
                var user = await _context.Users.FindAsync(nextReservation.UserId);
                var notification = new Notification
                {
                    Title = "Book Available",
                    Content = $"The book '{book.Title}' you reserved is now available!",
                    CreatedAt = DateTime.UtcNow,
                    Type = "Reservation",
                    IsRead = false
                };
                await _notificationService.CreateNotificationAsync(notification);

                _context.Loans.Remove(nextReservation);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsPaidAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.LoanDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLoanAsync(Loan loan)
        {
            var existingLoan = await _context.Loans.FindAsync(loan.BookLoanId);
            if (existingLoan == null) return false;

            // Validate LoanDate to ensure it won't cause an overflow when calculating the due date
            if (loan.LoanDate.HasValue && (loan.LoanDate < DateTime.MinValue || loan.LoanDate > DateTime.MaxValue.AddDays(-_loanSettings.LoanDeferredTime)))
            {
                throw new ArgumentException("LoanDate is out of range and would cause an overflow when calculating the due date.");
            }

            existingLoan.LoanDate = loan.LoanDate;
            existingLoan.ReturnDate = loan.ReturnDate;

            _context.Loans.Update(existingLoan);
            await _context.SaveChangesAsync();
            return true;
        }

        public decimal? GetLoanCost(Loan loan)
        {
            var overdueDays = GetLoanDue(loan);
            if (overdueDays != null)
            {
                return -overdueDays * _loanSettings.LoanCostPerDay; // Calculate overdue fee
            }
            return null;
        }

        public int? GetLoanDue(Loan loan)
        {
            if (loan.LoanDate.HasValue && loan.LoanDate.Value.AddDays(_loanSettings.LoanDeferredTime) < DateTime.Now)
            {
                var overdueDays = (loan.LoanDate.Value.AddDays(_loanSettings.LoanDeferredTime) - DateTime.Now).Days;
                return overdueDays;
            }
            return 0;
        }

        public DateTime? GetLoanDueDate(Loan loan)
        {
            if (!loan.LoanDate.HasValue)
            {
                Console.WriteLine($"Loan {loan.BookLoanId}: LoanDate is null.");
                return null;
            }

            Console.WriteLine($"Loan {loan.BookLoanId}: LoanDate = {loan.LoanDate.Value}, LoanDeferredTime = {_loanSettings.LoanDeferredTime}");

            // Calculate the maximum number of days that can be added without exceeding DateTime.MaxValue
            var maxDays = (DateTime.MaxValue - loan.LoanDate.Value).TotalDays;
            if (_loanSettings.LoanDeferredTime > maxDays)
            {
                Console.WriteLine($"Loan {loan.BookLoanId}: Adding {_loanSettings.LoanDeferredTime} days to {loan.LoanDate.Value} would exceed DateTime.MaxValue. Returning DateTime.MaxValue.");
                return DateTime.MaxValue;
            }

            return loan.LoanDate.Value.AddDays(_loanSettings.LoanDeferredTime);
        }
    }
}