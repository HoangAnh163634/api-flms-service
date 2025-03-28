using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Service
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _dbContext;

        public BookService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            try
            {
                var books = await _dbContext.Books
                    .Include(b => b.Author)
                    .Include(b => b.BookCategories)
                        .ThenInclude(bc => bc.Category)
                    .ToListAsync();

                Console.WriteLine($"GetAllBooksAsync: Retrieved {books.Count} books.");
                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBooksAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            try
            {
                var book = await _dbContext.Books
                    .Include(b => b.Author)
                    .Include(b => b.BookCategories)
                        .ThenInclude(bc => bc.Category)
                    .Include(b => b.Reviews)
                        .ThenInclude(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    Console.WriteLine($"GetBookByIdAsync: Book with ID {id} not found.");
                }
                return book;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookByIdAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book != null)
            {
                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
        {
            return await _dbContext.Books
                .Where(b => b.UserId == userId)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    BorrowedUntil = b.BorrowedUntil,
                    ImageUrls = b.ImageUrls
                })
                .ToListAsync();
        }

        public async Task<IActionResult> RenewBookAsync(int userId, int bookId)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId);

            if (book == null)
            {
                return new NotFoundObjectResult("Book not found or not borrowed by this user");
            }

            if (book.BorrowedUntil <= DateTime.Now)
            {
                return new BadRequestObjectResult("Cannot renew. The book's due date has already passed.");
            }

            book.BorrowedUntil = book.BorrowedUntil.AddDays(3);
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(book.BorrowedUntil, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            book.BorrowedUntil = vietnamTime;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Book renewed successfully", newDueDate = vietnamTime.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public async Task<Book> LoanBookAsync(int bookId, int userId)
        {
            var book = await _dbContext.Books
                .Include(b => b.BookLoans)
                .SingleOrDefaultAsync(b => b.BookId == bookId);
            if (book == null)
            {
                Console.WriteLine($"Book with ID {bookId} not found.");
                return null;
            }

            if (book.AvailableCopies <= 0)
            {
                Console.WriteLine($"Book with ID {bookId} is not available for loan.");
                return null;
            }

            var user = await _dbContext.Users
                .Include(u => u.BookLoans)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                Console.WriteLine($"User with ID {userId} not found.");
                return null;
            }

            if (book.BookLoans.Any(l => l.UserId == userId && l.ReturnDate == null))
            {
                Console.WriteLine($"User {userId} has already loaned book {bookId}.");
                return null;
            }

            var loan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(14)
            };

            book.AvailableCopies -= 1;
            _dbContext.Loans.Add(loan);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"Book {bookId} loaned to user {userId} successfully.");
            return book;
        }

        public async Task<string> ReserveBookAsync(int bookId, int userId)
        {
            var book = await _dbContext.Books
                .Include(b => b.BookLoans)
                .SingleOrDefaultAsync(b => b.BookId == bookId);
            if (book == null)
            {
                return "Book not found.";
            }

            if (book.AvailableCopies > 0)
            {
                return "Book is still available for loan.";
            }

            var existingReservation = await _dbContext.Loans
                .AnyAsync(l => l.BookId == bookId && l.UserId == userId && l.LoanDate == DateTime.MaxValue && l.ReturnDate == null);
            if (existingReservation)
            {
                return "You have already reserved this book.";
            }

            var reservation = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.MaxValue,
                ReturnDate = null
            };
            _dbContext.Loans.Add(reservation);
            await _dbContext.SaveChangesAsync();

            return "Book reserved successfully!";
        }

        public async Task<List<Book>> GetReservedBooksAsync(int userId)
        {
            return await _dbContext.Loans
                .Where(l => l.UserId == userId && l.LoanDate == DateTime.MaxValue && l.ReturnDate == null)
                .Include(l => l.Book)
                .Select(l => new Book
                {
                    BookId = l.Book.BookId,
                    Title = l.Book.Title,
                    ImageUrls = l.Book.ImageUrls,
                    BookDescription = l.Book.BookDescription
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm, string categoryName)
        {
            var query = _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.Title != null && b.Title.Contains(searchTerm)) ||
                                         (b.BookDescription != null && b.BookDescription.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(b => b.BookCategories.Any(bc => EF.Functions.ILike(bc.Category.CategoryName ?? "", categoryName)));
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AuthorExistsAsync(int authorId)
        {
            return await _dbContext.Authors.AnyAsync(a => a.AuthorId == authorId);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _dbContext.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }
    }
}