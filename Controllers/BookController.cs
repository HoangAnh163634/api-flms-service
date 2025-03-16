using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly AuthService _authService;

        public BookController(IBookService bookService, IUserService userService, AuthService authService)
        {
            _bookService = bookService;
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>List of books with their authors and categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                var bookDtos = books.Select(b => new BookDto
                {
                    BookId = b.BookId,
                    BookName = b.Title ?? "No Title",
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author?.Name ?? "No Author",
                    Category = b.BookCategories?.Select(bc => bc.Category).ToList() ?? new List<Category>(),
                    BookNo = b.ISBN ?? "No ISBN",
                    BookPrice = b.PublicationYear,
                    AvailableCopies = b.AvailableCopies,
                    BookDescription = b.BookDescription,
                    CloudinaryImageId = b.CloudinaryImageId,
                    ImageUrls = b.ImageUrls
                }).ToList();

                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBooks: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while retrieving books.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Book details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                var bookDto = new BookDto
                {
                    BookId = book.BookId,
                    BookName = book.Title,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author?.Name ?? "No Author",
                    Category = book.BookCategories?.Select(bc => bc.Category).ToList() ?? new List<Category>(),
                    BookNo = book.ISBN,
                    BookPrice = book.PublicationYear,
                    AvailableCopies = book.AvailableCopies,
                    BookDescription = book.BookDescription,
                    CloudinaryImageId = book.CloudinaryImageId,
                    ImageUrls = book.ImageUrls
                };

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Add a new book
        /// </summary>
        /// <param name="bookDto">Book DTO</param>
        /// <param name="images">List of images</param>
        /// <returns>Created book</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] BookDto bookDto, [FromForm] List<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var book = new Book
                {
                    Title = bookDto.BookName,
                    AuthorId = bookDto.AuthorId,
                    ISBN = bookDto.BookNo,
                    PublicationYear = bookDto.BookPrice,
                    AvailableCopies = bookDto.AvailableCopies,
                    BookDescription = bookDto.BookDescription
                    // Category không ánh xạ trực tiếp ở đây, sẽ xử lý trong service nếu cần
                };

                var createdBook = await _bookService.CreateBookAsync(book, images);

                var createdBookDto = new BookDto
                {
                    BookId = createdBook.BookId,
                    BookName = createdBook.Title,
                    AuthorId = createdBook.AuthorId,
                    AuthorName = createdBook.Author?.Name ?? "No Author",
                    Category = createdBook.BookCategories?.Select(bc => bc.Category).ToList() ?? new List<Category>(),
                    BookNo = createdBook.ISBN,
                    BookPrice = createdBook.PublicationYear,
                    AvailableCopies = createdBook.AvailableCopies,
                    BookDescription = createdBook.BookDescription,
                    CloudinaryImageId = createdBook.CloudinaryImageId,
                    ImageUrls = createdBook.ImageUrls
                };

                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.BookId }, createdBookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing book
        /// </summary>
        /// <param name="bookDto">Updated book DTO</param>
        /// <param name="images">List of images</param>
        /// <returns>Updated book</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromForm] BookDto bookDto, [FromForm] List<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingBook = await _bookService.GetBookByIdAsync(bookDto.BookId);
                if (existingBook == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                existingBook.Title = bookDto.BookName;
                existingBook.AuthorId = bookDto.AuthorId;
                existingBook.ISBN = bookDto.BookNo;
                existingBook.PublicationYear = bookDto.BookPrice;
                existingBook.AvailableCopies = bookDto.AvailableCopies;
                existingBook.BookDescription = bookDto.BookDescription;
                // Category không ánh xạ trực tiếp ở đây, sẽ xử lý trong service nếu cần

                var updatedBook = await _bookService.UpdateBookAsync(existingBook, images);

                var updatedBookDto = new BookDto
                {
                    BookId = updatedBook.BookId,
                    BookName = updatedBook.Title,
                    AuthorId = updatedBook.AuthorId,
                    AuthorName = updatedBook.Author?.Name ?? "No Author",
                    Category = updatedBook.BookCategories?.Select(bc => bc.Category).ToList() ?? new List<Category>(),
                    BookNo = updatedBook.ISBN,
                    BookPrice = updatedBook.PublicationYear,
                    AvailableCopies = updatedBook.AvailableCopies,
                    BookDescription = updatedBook.BookDescription,
                    CloudinaryImageId = updatedBook.CloudinaryImageId,
                    ImageUrls = updatedBook.ImageUrls
                };

                return Ok(updatedBookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Book not found" });
                }

                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get borrowed books for the logged-in user
        /// </summary>
        /// <returns>List of borrowed books</returns>
        [HttpGet("borrowed")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var userOfGG = await _authService.GetCurrentUserAsync();

            // Kiểm tra nếu người dùng chưa đăng nhập (userOfGG là null)
            if (userOfGG == null)
            {
                return Unauthorized("User is not logged in");
            }

            var user = await _userService.GetUserByEmail(userOfGG.Email);

            // Kiểm tra nếu không có user
            if (user == null || user.UserId <= 0)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Kiểm tra quyền của người dùng
            if (user.Role != "User" && user.Role != "Admin")
            {
                return RedirectToPage("/AccessDenied");
            }

            var books = await _bookService.GetBorrowedBooksAsync(user.UserId);
            return Ok(books);
        }

        /// <summary>
        /// Renew a book
        /// </summary>
        /// <param name="request">Renew request containing BookId</param>
        /// <returns>Result of renewal</returns>
        [HttpPost("renew")]
        public async Task<IActionResult> RenewBook([FromBody] RenewRequest request)
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            var user = await _userService.GetUserByEmail(userOfGG?.Email); // Kiểm tra null cho userOfGG
            if (user == null || user.UserId <= 0)
            {
                return Unauthorized("User is not logged in");
            }

            // Kiểm tra nếu BookId không hợp lệ
            if (request.BookId <= 0)
            {
                return BadRequest("Invalid ID");
            }

            // Gọi service gia hạn sách
            var result = await _bookService.RenewBookAsync(user.UserId, request.BookId);
            return result;
        }

        /// <summary>
        /// Search books by name, author, and category
        /// </summary>
        /// <param name="bookName">Book name to search</param>
        /// <param name="authorName">Author name to search</param>
        /// <param name="categoryId">Category ID to search</param>
        /// <returns>List of matching books</returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search(string bookName, string authorName, string categoryId)
        {
            // Lọc sách theo tên sách, tên tác giả, và thể loại (nếu có)
            var books = await _bookService.SearchBooks(bookName, authorName, categoryId);
            return Ok(books);
        }
    }
}