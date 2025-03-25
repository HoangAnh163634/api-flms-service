using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Service
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;

        public LoanService(AppDbContext context)
        {
            _context = context;
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
            var book = await _context.Books.FindAsync(loan.BookId);
            if (book == null || book.AvailableCopies <= 0)
                throw new Exception("Book is not available");

            book.AvailableCopies--;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> ReturnLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;
            if (loan.ReturnDate != null) return false;

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
                book.AvailableCopies++;
            loan.ReturnDate = DateTime.Now;
            
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
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

    }
}
