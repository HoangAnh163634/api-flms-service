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
                .Include(b => b.Category)
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

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
        {
            return await _dbContext.Books
                                    .Where(b => b.UserId == userId)
                                    .Select(b => new Book
                                    {
                                        BookId = b.BookId,
                                        BookName = b.BookName,
                                        BorrowedUntil = b.BorrowedUntil,
                                        // Lấy các đường dẫn ảnh từ cơ sở dữ liệu
                                        ImageUrls = b.ImageUrls // Giả sử ImageUrls là trường lưu trữ chuỗi các đường dẫn ảnh
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

          
            if (book.BorrowedUntil == DateTime.MinValue)
            {
            
                book.BorrowedUntil = DateTime.UtcNow;
            }

         
            book.BorrowedUntil = book.BorrowedUntil.AddDays(3);

    
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(book.BorrowedUntil, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            book.BorrowedUntil = vietnamTime;

      
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Book renewed successfully", newDueDate = vietnamTime.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string? bookName, string? authorName, int? categoryId, int? minPrice, int? maxPrice)
        {
         
            IQueryable<Book> query = _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category);

           
            if (!string.IsNullOrEmpty(bookName))
            {
                query = query.Where(b => b.BookName.Contains(bookName));
            }

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.AuthorName.Contains(authorName));
            }

          
            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CatId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.BookPrice >= minPrice.Value);
            }

           
            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.BookPrice <= maxPrice.Value);
            }

            
            return await query.OrderBy(b => b.BookName).ToListAsync();
        }


    }
}
