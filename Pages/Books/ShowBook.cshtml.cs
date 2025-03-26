using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using api_flms_service.Entity;
using api_auth_service.Services;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Pages.Books
{
    public class ShowBookModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly AppDbContext _dbContext;

        public ShowBookModel(IBookService bookService, AuthService authService, IUserService userService, IReviewService reviewService, AppDbContext dbContext)
        {
            _authService = authService;
            _bookService = bookService;
            _userService = userService;
            _reviewService = reviewService;
            _dbContext = dbContext; 
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public User User { get; set; } = default!;
        public string role { get; set; } = "guest";
        [BindProperty]
        public string errorMessage { get; set; } = "";
        [BindProperty]
        public int SomeValue { get; set; }
        [BindProperty]
        public bool wasLoaned { get; set; } = false;
        [BindProperty]
        public bool wasReviewed { get; set; } = false;

        [BindProperty]
        public Review NewReview { get; set; } = new Review();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SomeValue = (int)id;
            var book = await _bookService.GetBookByIdAsync((int)id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                // Tải lại BookCategories vì GetBookByIdAsync không tải đúng
                /*book.BookCategories = await _dbContext.BookCategories
                    .Include(bc => bc.Category)
                    .Where(bc => bc.BookId == book.BookId)
                    .ToListAsync();
                Book = book;*/
                Book = book;
                await Console.Out.WriteLineAsync($"Book ID: {id}, AvailableCopies: {Book.AvailableCopies}");
                await Console.Out.WriteLineAsync("Count: " + Book.Reviews.Count() + " Number");
                // Thêm logging cho Categories
                await Console.Out.WriteLineAsync("Categories Count: " + Book.Categories.Count());
                foreach (var category in Book.Categories)
                {
                    await Console.Out.WriteLineAsync($"Category: {category.CategoryName}");
                }
            }

            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail != null)
            {
                User = await _userService.GetUserByEmail(userEmail);
                role = User?.Role ?? "guest";

                if (User != null)
                {
                    foreach (var bookloan in User.BookLoans)
                    {
                        if (bookloan.Book != null && bookloan.Book.BookId == id)
                        {
                            if (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow)
                            {
                                wasLoaned = true;
                                break;
                            }
                        }
                    }

                    if (Book.Reviews != null && Book.Reviews.Any(r => r.UserId == User.UserId))
                    {
                        wasReviewed = true;
                    }
                }
            }

            NewReview.BookId = (int)id;
            if (User != null) NewReview.UserId = User.UserId;

            await Console.Out.WriteLineAsync($"Role: {role}, WasLoaned: {wasLoaned}, WasReviewed: {wasReviewed}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            int someValue = int.Parse(Request.Form["someValue"]);

            // Tải lại thông tin user để đảm bảo role được cập nhật
            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail != null)
            {
                User = await _userService.GetUserByEmail(userEmail);
                role = User?.Role ?? "guest";
            }
            else
            {
                ModelState.AddModelError("", "You must be logged in to perform this action.");
                Book = await _bookService.GetBookByIdAsync(someValue);
                return Page();
            }

            if (handler == "handler1")
            {
                var userOfGG = await _authService.GetCurrentUserAsync();
                if (userOfGG == null || string.IsNullOrEmpty(userOfGG.Email))
                {
                    ModelState.AddModelError("", "You must be logged in to loan a book.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                var user = await _userService.GetUserByEmail(userOfGG.Email);
                if (user == null || user.Role != "User")
                {
                    ModelState.AddModelError("", "Only users with role 'User' can loan books.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                var result = await _bookService.LoanBookAsync(someValue, user.UserId);
                if (result == null)
                {
                    ModelState.AddModelError("", "Failed to loan the book. It may not be available or you have already loaned it.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                TempData["SuccessMessage"] = "Book loaned successfully!";
                return RedirectToPage(new { id = someValue });
            }
            else if (handler == "addReview")
            {
                // Xóa các lỗi validation không liên quan đến NewReview
                var keysToRemove = ModelState.Keys.Where(k => !k.StartsWith("NewReview")).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }

                // Validate chỉ các trường của NewReview
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    Book = await _bookService.GetBookByIdAsync(someValue);

                    // Cập nhật lại wasLoaned và wasReviewed để hiển thị form
                    if (User != null)
                    {
                        foreach (var bookloan in User.BookLoans)
                        {
                            if (bookloan.Book != null && bookloan.Book.BookId == someValue)
                            {
                                if (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow)
                                {
                                    wasLoaned = true;
                                    break;
                                }
                            }
                        }

                        if (Book.Reviews != null && Book.Reviews.Any(r => r.UserId == User.UserId))
                        {
                            wasReviewed = true;
                        }
                    }

                    await Console.Out.WriteLineAsync($"Validation failed - Role: {role}, WasLoaned: {wasLoaned}, WasReviewed: {wasReviewed}");
                    return Page();
                }

                // Kiểm tra role trước khi thêm review
                if (role != "User")
                {
                    ModelState.AddModelError("", "Only users with role 'User' can add reviews.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                if (wasReviewed)
                {
                    ModelState.AddModelError("", "You have already reviewed this book.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                NewReview.ReviewDate = DateTime.Now;
                var addedReview = await _reviewService.AddReviewAsync(NewReview);
                if (addedReview == null)
                {
                    ModelState.AddModelError("", "Failed to add review.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                TempData["SuccessMessage"] = "Review added successfully!";
                return RedirectToPage(new { id = someValue });
            }

            Book = await _bookService.GetBookByIdAsync(someValue);
            if (Book == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}