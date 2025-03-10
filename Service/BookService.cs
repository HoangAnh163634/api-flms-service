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
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Reload the book with related data
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            // Reload the updated book with related data
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
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

        // Lấy danh sách sách mượn của người dùng
        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
        {
            return await _dbContext.Books
                                 .Where(b => b.UserId == userId)
                                 .Select(b => new Book
                                 {
                                     BookId = b.BookId,
                                     BookName = b.BookName,
                                     BorrowedUntil = b.BorrowedUntil
                                 })
                                 .ToListAsync();
        }
        //// Gia hạn sách
        //public async Task<IActionResult> RenewBookAsync(int userId, int bookId)
        //{
        //    // Tìm sách mượn của người dùng
        //    var book = await _dbContext.Books
        //                             .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == 1);

        //    if (book == null)
        //    {
        //        return new NotFoundObjectResult("Book not found or not borrowed by this user");
        //    }

        //    // Kiểm tra nếu sách có thể gia hạn
        //    if (book.BorrowedUntil < DateTime.Now)
        //    {
        //        return new BadRequestObjectResult("Cannot renew this book. The borrowing period has already ended.");
        //    }

        //    // Gia hạn sách + 3 ngày
        //    book.BorrowedUntil = book.BorrowedUntil.AddDays(3);
        //    _dbContext.Books.Update(book);
        //    await _dbContext.SaveChangesAsync();

        //    return new OkObjectResult(new { message = "Book renewed successfully", newDueDate = book.BorrowedUntil });
        //}

        public async Task<IActionResult> RenewBookAsync(int userId, int bookId)
        {
            // Tìm sách mượn của người dùng
            var book = await _dbContext.Books
                                     .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId);

            if (book == null)
            {
                return new NotFoundObjectResult("Book not found or not borrowed by this user");
            }

            // Kiểm tra nếu BorrowedUntil có giá trị mặc định (0001-01-01), nếu có thì gán giá trị là thời gian hiện tại (UTC)
            if (book.BorrowedUntil == DateTime.MinValue)
            {
                // Gán thời gian hiện tại nếu BorrowedUntil có giá trị mặc định
                book.BorrowedUntil = DateTime.UtcNow;
            }

            // Cộng thêm 3 ngày vào BorrowedUntil
            book.BorrowedUntil = book.BorrowedUntil.AddDays(3);

            // Chuyển đổi thời gian sang giờ Việt Nam (UTC+7)
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(book.BorrowedUntil, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Lưu lại giá trị mới vào cơ sở dữ liệu
            book.BorrowedUntil = vietnamTime;

            // Cập nhật sách và lưu thay đổi vào cơ sở dữ liệu
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            // Trả kết quả với thời gian gia hạn
            return new OkObjectResult(new { message = "Book renewed successfully", newDueDate = vietnamTime.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string? bookName, string? authorName, int? categoryId, int? minPrice, int? maxPrice)
        {
            // Khởi tạo query cơ bản
            IQueryable<Book> query = _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category);

            // Nếu bookName có giá trị, áp dụng điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(bookName))
            {
                query = query.Where(b => b.BookName.Contains(bookName));
            }

            // Nếu authorName có giá trị, áp dụng điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.AuthorName.Contains(authorName));
            }

            // Nếu categoryId có giá trị, áp dụng điều kiện tìm kiếm
            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CatId == categoryId.Value);
            }

            // Nếu minPrice có giá trị, áp dụng điều kiện tìm kiếm
            if (minPrice.HasValue)
            {
                query = query.Where(b => b.BookPrice >= minPrice.Value);
            }

            // Nếu maxPrice có giá trị, áp dụng điều kiện tìm kiếm
            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.BookPrice <= maxPrice.Value);
            }

            // Thực thi truy vấn và trả về kết quả
            return await query.OrderBy(b => b.BookName).ToListAsync();
        }


    }
}
