using System.Net;
using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly ICloudinaryService _cloudinaryService;

        public BookController(IBookService bookService, IUserService userService, AuthService authService, ICloudinaryService cloudinaryService)
        {
            _bookService = bookService;
            _userService = userService;
            _authService = authService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                if (books == null || !books.Any())
                {
                    Console.WriteLine("No books found in the database.");
                    return Ok(new List<BookDto>());
                }

                var bookDtos = books.Select(b => new BookDto
                {
                    BookId = b.BookId,
                    BookName = b.Title ?? "No Title",
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author?.Name ?? "No Author",
                    Categories = b.BookCategories?.Select(bc => new CategoryDto
                    {
                        CategoryId = bc.Category.CategoryId,
                        CategoryName = bc.Category.CategoryName
                    }).ToList() ?? new List<CategoryDto>(),
                    BookNo = b.ISBN ?? "No ISBN",
                    BookPrice = b.PublicationYear,
                    AvailableCopies = b.AvailableCopies,
                    BookDescription = b.BookDescription,
                    CloudinaryImageId = b.CloudinaryImageId,
                    ImageUrls = b.ImageUrls,
                    BookFileUrl = b.BookFileUrl
                }).ToList();

                Console.WriteLine($"Successfully retrieved {bookDtos.Count} books.");
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBooks: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving books.",
                    Details = ex.Message
                });
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
                    return NotFound(new ErrorResponse { Message = "Book not found" });
                }

                var bookDto = new BookDto
                {
                    BookId = book.BookId,
                    BookName = book.Title,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author?.Name ?? "No Author",
                    Categories = book.BookCategories?.Select(bc => new CategoryDto
                    {
                        CategoryId = bc.Category.CategoryId,
                        CategoryName = bc.Category.CategoryName
                    }).ToList() ?? new List<CategoryDto>(),
                    BookNo = book.ISBN,
                    BookPrice = book.PublicationYear,
                    AvailableCopies = book.AvailableCopies,
                    BookDescription = book.BookDescription,
                    CloudinaryImageId = book.CloudinaryImageId,
                    ImageUrls = book.ImageUrls,
                    BookFileUrl = book.BookFileUrl
                };

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving the book.",
                    Details = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] BookRequestDto bookDto, IFormFile? bookFile)
        {
            if (!ModelState.IsValid)
            {
                // Log chi tiết lỗi ModelState
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                Console.WriteLine("ModelState Errors: " + JsonConvert.SerializeObject(errors));
                return BadRequest(new ErrorResponse
                {
                    Message = "Invalid data submitted.",
                    Details = JsonConvert.SerializeObject(errors)
                });
            }

            try
            {
                var book = new Book
                {
                    Title = bookDto.BookName,
                    AuthorId = bookDto.AuthorId,
                    ISBN = bookDto.BookNo,
                    PublicationYear = (int)bookDto.BookPrice,
                    AvailableCopies = bookDto.AvailableCopies,
                    BookDescription = bookDto.BookDescription,
                    BookFileUrl = bookDto.BookFileUrl
                };

                if (bookFile != null)
                {
                    try
                    {
                        // Upload file hình ảnh lên Cloudinary và lưu URL vào ImageUrls
                        var imageUrl = await _cloudinaryService.UploadFileAsync(bookFile);
                        book.ImageUrls = imageUrl; // Lưu URL hình ảnh vào ImageUrls
                    }
                    catch (Exception uploadEx)
                    {
                        return StatusCode(500, new ErrorResponse
                        {
                            Message = "Error uploading file to Cloudinary",
                            Details = uploadEx.Message
                        });
                    }
                }

                var createdBook = await _bookService.CreateBookAsync(book);

                if (bookDto.CategoryIds != null && bookDto.CategoryIds.Any())
                {
                    createdBook.BookCategories = bookDto.CategoryIds.Select(id => new BookCategory
                    {
                        BookId = createdBook.BookId,
                        CategoryId = id
                    }).ToList();

                    await _bookService.UpdateBookAsync(createdBook);
                }

                var createdBookDto = new BookRequestDto
                {
                    BookId = createdBook.BookId,
                    BookName = createdBook.Title,
                    AuthorId = createdBook.AuthorId,
                    AuthorName = createdBook.Author?.Name ?? "No Author",
                    CategoryIds = createdBook.BookCategories?.Select(bc => bc.CategoryId).ToList(),
                    BookNo = createdBook.ISBN,
                    BookPrice = createdBook.PublicationYear,
                    AvailableCopies = createdBook.AvailableCopies,
                    BookDescription = createdBook.BookDescription,
                    BookFileUrl = createdBook.BookFileUrl,
                    ImageUrls = createdBook.ImageUrls?.Split(", ").ToList()
                };

                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.BookId }, createdBookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while creating the book.",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookRequestDto bookDto, IFormFile? bookFile)
        {
            Console.WriteLine($"UpdateBook called with id: {id}, bookDto.BookId: {bookDto.BookId}");
            if (id != bookDto.BookId)
            {
                return BadRequest(new ErrorResponse { Message = "Book ID in URL does not match Book ID in form data" });
            }

            if (!ModelState.IsValid)
            {
                // Log chi tiết lỗi ModelState
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                Console.WriteLine("ModelState Errors: " + JsonConvert.SerializeObject(errors));
                return BadRequest(new ErrorResponse
                {
                    Message = "Invalid data submitted.",
                    Details = JsonConvert.SerializeObject(errors)
                });
            }

            try
            {
                var existingBook = await _bookService.GetBookByIdAsync(bookDto.BookId);
                if (existingBook == null)
                {
                    return NotFound(new ErrorResponse { Message = "Book not found" });
                }

                existingBook.Title = bookDto.BookName;
                existingBook.AuthorId = bookDto.AuthorId;
                existingBook.ISBN = bookDto.BookNo;
                existingBook.PublicationYear = (int)bookDto.BookPrice;
                existingBook.AvailableCopies = bookDto.AvailableCopies;
                existingBook.BookDescription = bookDto.BookDescription ?? existingBook.BookDescription;

                if (bookFile != null)
                {
                    try
                    {
                        // Upload file hình ảnh lên Cloudinary và lưu URL vào ImageUrls
                        var imageUrl = await _cloudinaryService.UploadFileAsync(bookFile);
                        existingBook.ImageUrls = imageUrl; // Lưu URL hình ảnh vào ImageUrls
                    }
                    catch (Exception uploadEx)
                    {
                        return StatusCode(500, new ErrorResponse
                        {
                            Message = "Error uploading file to Cloudinary",
                            Details = uploadEx.Message
                        });
                    }
                }

                if (bookDto.CategoryIds != null && bookDto.CategoryIds.Any())
                {
                    var existingCategories = existingBook.BookCategories.Select(c => c.CategoryId).ToHashSet();
                    var newCategories = bookDto.CategoryIds.ToHashSet();

                    var categoriesToRemove = existingCategories.Except(newCategories).ToList();
                    var categoriesToAdd = newCategories.Except(existingCategories).ToList();

                    var itemsToRemove = existingBook.BookCategories
                        .Where(c => categoriesToRemove.Contains(c.CategoryId))
                        .ToList();

                    foreach (var item in itemsToRemove)
                    {
                        existingBook.BookCategories.Remove(item);
                    }

                    foreach (var categoryId in categoriesToAdd)
                    {
                        existingBook.BookCategories.Add(new BookCategory
                        {
                            BookId = bookDto.BookId,
                            CategoryId = categoryId
                        });
                    }
                }

                await _bookService.UpdateBookAsync(existingBook);
                return Ok(new { message = "Book updated successfully", bookId = existingBook.BookId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBook: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while updating the book.",
                    Details = ex.Message
                });
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
                    return NotFound(new ErrorResponse { Message = "Book not found" });
                }

                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while deleting the book.",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("borrowed")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            if (userOfGG == null)
            {
                return Unauthorized(new ErrorResponse { Message = "User is not logged in" });
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
                return Unauthorized(new ErrorResponse { Message = "User is not logged in" });
            }

            if (request.BookId <= 0)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid ID" });
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

        [HttpPost("reserve/{bookId}")]
        public async Task<IActionResult> ReserveBook(int bookId)
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            if (userOfGG == null)
                return Unauthorized(new ErrorResponse { Message = "User is not logged in" });

            var user = await _userService.GetUserByEmail(userOfGG.Email);
            if (user == null || user.Role != "User")
                return Unauthorized(new ErrorResponse { Message = "Only users with role 'User' can reserve books" });

            var result = await _bookService.ReserveBookAsync(bookId, user.UserId);
            if (result.Contains("successfully"))
            {
                return Ok(new { message = result });
            }
            return BadRequest(new ErrorResponse { Message = result });
        }

        [HttpGet("reserved")]
        public async Task<IActionResult> GetReservedBooks()
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            if (userOfGG == null)
                return Unauthorized(new ErrorResponse { Message = "User is not logged in" });

            var user = await _userService.GetUserByEmail(userOfGG.Email);
            if (user == null || user.UserId <= 0)
                return Unauthorized(new ErrorResponse { Message = "Invalid user" });

            var books = await _bookService.GetReservedBooksAsync(user.UserId);
            return Ok(books);
        }

        private readonly List<string> _allowedExtensions = new() { ".pdf", ".epub", ".mobi", ".png", ".jpg" };

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ErrorResponse { Message = "No file uploaded." });
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid file format. Allowed formats: PDF, EPUB, MOBI, PNG, JPG." });
            }

            var uploadResult = await _cloudinaryService.UploadFileAsync(file);
            return Ok(new { fileUrl = uploadResult });
        }

        [HttpGet("get/{publicId}")]
        public async Task<IActionResult> GetFile(string publicId)
        {
            var file = await _cloudinaryService.GetFileAsync(publicId);
            if (file == null)
            {
                return NotFound(new ErrorResponse { Message = "File not found." });
            }
            return Ok(file);
        }

        [HttpDelete("delete/{publicId}")]
        public async Task<IActionResult> DeleteFile(string publicId)
        {
            var result = await _cloudinaryService.DeleteFileAsync(publicId);
            if (result == null || result.Result != "ok")
            {
                return NotFound(new ErrorResponse { Message = "File not found or could not be deleted." });
            }
            return Ok(new ErrorResponse { Message = "File deleted successfully." });
        }
    }
}