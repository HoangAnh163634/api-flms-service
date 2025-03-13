using api_flms_service.Entity;
using api_flms_service.Model;
using global::api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;


namespace api_flms_service.Services
{
    public class IssuedBookService : IIssuedBookService
    {
        private readonly AppDbContext _dbContext;

        public IssuedBookService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IssuedBook>> GetAllIssuedBooksAsync()
        {
            return await _dbContext.IssuedBooks.ToListAsync();
        }

        public async Task<IssuedBook?> GetIssuedBookByIdAsync(int id)
        {
            return await _dbContext.IssuedBooks.FindAsync(id);
        }

        public async Task<IssuedBook> AddIssuedBookAsync(IssuedBook issuedBook)
        {
            _dbContext.IssuedBooks.Add(issuedBook);
            await _dbContext.SaveChangesAsync();
            return issuedBook;
        }

        public async Task<IssuedBook?> UpdateIssuedBookAsync(IssuedBook issuedBook)
        {
            var existingIssuedBook = await _dbContext.IssuedBooks.FindAsync(issuedBook.SNo);
            if (existingIssuedBook == null) return null;

            existingIssuedBook.BookNo = issuedBook.BookNo;
            existingIssuedBook.BookName = issuedBook.BookName;
            existingIssuedBook.BookAuthor = issuedBook.BookAuthor;
            existingIssuedBook.StudentId = issuedBook.StudentId;
            existingIssuedBook.Status = issuedBook.Status;
            existingIssuedBook.IssueDate = issuedBook.IssueDate;

            await _dbContext.SaveChangesAsync();
            return existingIssuedBook;
        }

        public async Task<bool> DeleteIssuedBookAsync(int id)
        {
            var issuedBook = await _dbContext.IssuedBooks.FindAsync(id);
            if (issuedBook == null) return false;

            _dbContext.IssuedBooks.Remove(issuedBook);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}


