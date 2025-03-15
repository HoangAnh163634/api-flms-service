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
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .Include(b => b.Reviews)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(b => b.BookId == id);


        }

        public async Task<Book> CreateBookAsync(Book book, List<IFormFile> images)
        {
            var uploadedImagePaths = new List<string>();

            // Định nghĩa đường dẫn thư mục lưu ảnh
            var imageFolderPath = Path.Combine("wwwroot", "images");

            // Kiểm tra nếu thư mục không tồn tại thì tạo thư mục
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

            foreach (var image in images)
            {
                // Đường dẫn để lưu tệp ảnh
                var filePath = Path.Combine(imageFolderPath, image.FileName);

                // Lưu ảnh vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Thêm đường dẫn ảnh vào danh sách
                uploadedImagePaths.Add("/images/" + image.FileName);
            }

            // Nối các đường dẫn ảnh thành một chuỗi phân tách bởi dấu phẩy
            var imageUrls = string.Join(",", uploadedImagePaths);

            // Cập nhật đường dẫn ảnh vào Book object
            book.ImageUrls = imageUrls;

            // Thêm sách vào cơ sở dữ liệu
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Trả về sách đã được thêm vào cơ sở dữ liệu
            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);
        }



        public async Task<Book> UpdateBookAsync(Book book, List<IFormFile> images)
        {
            
            var uploadedImagePaths = new List<string>();

            foreach (var image in images)
            {
                var filePath = Path.Combine("wwwroot", "images", image.FileName);

              
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

               
                uploadedImagePaths.Add("/images/" + image.FileName);
            }

            
            if (uploadedImagePaths.Any())
            {
                var existingImageUrls = book.ImageUrls.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                existingImageUrls.AddRange(uploadedImagePaths); 
                book.ImageUrls = string.Join(",", existingImageUrls);
            }

       
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
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

            // Kiểm tra nếu sách có thể gia hạn hay không (không quá hạn)
            if (book.BorrowedUntil <= DateTime.Now)
            {
                return new BadRequestObjectResult("Cannot renew. The book's due date has already passed.");
            }

            // Gia hạn sách thêm 3 ngày
            book.BorrowedUntil = book.BorrowedUntil.AddDays(3);

            // Chuyển thời gian về múi giờ Việt Nam
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(book.BorrowedUntil, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            book.BorrowedUntil = vietnamTime;

            // Cập nhật lại sách trong DB
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Book renewed successfully", newDueDate = vietnamTime.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public async Task<List<Book>> SearchBooks(string bookName, string authorName, string categoryId)
        {
            var query = _dbContext.Books.AsQueryable();

            // Tìm kiếm theo tên sách
            if (!string.IsNullOrEmpty(bookName))
            {
                query = query.Where(b => b.Title.Contains(bookName));
            }

            // Tìm kiếm theo tên tác giả
            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.Name.Contains(authorName));
            }

            // Tìm kiếm theo thể loại
            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(b => b.BookCategories.Any(bc => bc.CategoryId.ToString() == categoryId));
            }

            return await query.ToListAsync();
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

            // Kiểm tra xem user đã mượn sách này chưa
            if (book.BookLoans.Any(l => l.UserId == userId && l.ReturnDate == null))
            {
                Console.WriteLine($"User {userId} has already loaned book {bookId}.");
                return null;
            }

            // Tạo bản ghi mượn sách
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

    

        
    }
}
