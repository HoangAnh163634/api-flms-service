using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Services
{
    public class ReserveBookService : IReserveBookService
    {
        private readonly AppDbContext _dbContext;

        public ReserveBookService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(string authorFilter = null)
        {
            var books = _dbContext.Books.Include(b => b.Category).Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrEmpty(authorFilter))
            {
                books = books.Where(b => b.Author.AuthorName.Contains(authorFilter));
            }

            return await books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _dbContext.Books.Include(b => b.Category).Include(b => b.Author).FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateBookAsync(Book book)
        {
            var existingBook = await _dbContext.Books.FindAsync(book.BookId);
            if (existingBook == null) return null;

            existingBook.BookName = book.BookName;
            existingBook.AuthorId = book.AuthorId;
            existingBook.CatId = book.CatId;
            existingBook.BookNo = book.BookNo;
            existingBook.BookPrice = book.BookPrice;
            await _dbContext.SaveChangesAsync();
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null) return false;

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

