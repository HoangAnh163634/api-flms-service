using System.Net;
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

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] BookDto bookDto, [FromForm] List<IFormFile> images, [FromForm] List<int> categoryIds)
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
                    BookDescription = bookDto.BookDescription,
                    BookCategories = categoryIds.Select(id => new BookCategory { CategoryId = id }).ToList()
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookDto bookDto, [FromForm] List<IFormFile> images, [FromForm] List<int> categoryIds)
        {
            Console.WriteLine($"UpdateBook called with id: {id}, bookDto.BookId: {bookDto.BookId}"); // Debug: Kiểm tra request
            if (id != bookDto.BookId)
            {
                return BadRequest(new { message = "Book ID in URL does not match Book ID in form data" });
            }

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

                existingBook.BookCategories.Clear();
                existingBook.BookCategories = categoryIds.Select(cid => new BookCategory { BookId = bookDto.BookId, CategoryId = cid }).ToList();

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
                Console.WriteLine($"Error in UpdateBook: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while updating the book.", details = ex.Message });
            }
        }

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

        [HttpGet("borrowed")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var userOfGG = await _authService.GetCurrentUserAsync();

            if (userOfGG == null)
            {
                return Unauthorized("User is not logged in");
            }

            var user = await _userService.GetUserByEmail(userOfGG.Email);

            if (user == null || user.UserId <= 0)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (user.Role != "User" && user.Role != "Admin")
            {
                return RedirectToPage("/AccessDenied");
            }

            var books = await _bookService.GetBorrowedBooksAsync(user.UserId);
            return Ok(books);
        }

        [HttpPost("renew")]
        public async Task<IActionResult> RenewBook([FromBody] RenewRequest request)
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            var user = await _userService.GetUserByEmail(userOfGG?.Email);
            if (user == null || user.UserId <= 0)
            {
                return Unauthorized("User is not logged in");
            }

            if (request.BookId <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var result = await _bookService.RenewBookAsync(user.UserId, request.BookId);
            return result;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string searchTerm, string categoryName)
        {

            var books = await _bookService.SearchBooksAsync(searchTerm, categoryName);

            return Ok(books);
        }

        

    }
}