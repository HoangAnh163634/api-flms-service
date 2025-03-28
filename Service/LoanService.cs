﻿using api_flms_service.Entity;
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
        private readonly int Days;

        public LoanService(AppDbContext context, IConfiguration loanSettings)
        {
            _context = context;
            _config = loanSettings;
            Days = int.Parse(_config["LoanSettings:LoanDeferredTime"]);
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
            if (loan.ReturnDate == null && DateTime.Now > loan.LoanDate.Value.AddDays(Days)) return false;

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

        public async Task<bool> UpdateLoanAsync(Loan loan)
        {
            var existingLoan = await _context.Loans.FindAsync(loan.BookLoanId);
            if (existingLoan == null) return false;

            // Chỉ cập nhật LoanDate và ReturnDate
            existingLoan.LoanDate = loan.LoanDate;
            existingLoan.ReturnDate = loan.ReturnDate;

            _context.Loans.Update(existingLoan);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
