using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using api_flms_service.Entity;
using api_auth_service.Services;
using Microsoft.AspNetCore.Identity.Data;
using api_flms_service.Services;

namespace api_flms_service.Pages.Books
{
    public class ShowBookModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;

        public ShowBookModel(IBookService bookService, AuthService authService, IUserService userService, IReviewService reviewService)
        {
            _authService = authService;
            _bookService = bookService;
            _userService = userService;
            _reviewService = reviewService; // Chỉ khởi tạo một lần
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public User User { get; set; } = default!;
        public string role { get; set; } = "User"; // Khởi tạo mặc định
        [BindProperty]
        public string errorMessage { get; set; } = "";
        [BindProperty]
        public int SomeValue { get; set; }
        [BindProperty]
        public bool wasLoaned { get; set; } = false;
        [BindProperty]
        public string opinion { get; set; }
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
                Book = book;
                await Console.Out.WriteLineAsync("Count: " + Book.Reviews.Count() + " Number");
            }

            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail != null)
            {
                User = await _userService.GetUserByEmail(userEmail);
                role = User?.Role ?? "guest"; // Gán role từ User

                if (User != null)
                {
                    foreach (var bookloan in User.BookLoans)
                    {
                        if (bookloan.Book != null && bookloan.Book.BookId == id)
                        {
                            wasLoaned = true;
                            break; // Thoát vòng lặp khi tìm thấy
                        }
                    }

                    // Kiểm tra xem user đã review sách này chưa
                    if (Book.Reviews != null && Book.Reviews.Any(r => r.UserId == User.UserId))
                    {
                        wasReviewed = true;
                    }
                }
            }

            // Khởi tạo NewReview với BookId và UserId mặc định
            NewReview.BookId = (int)id;
            if (User != null) NewReview.UserId = User.UserId;

            // Log để debug
            await Console.Out.WriteLineAsync($"Role: {role}, WasLoaned: {wasLoaned}, WasReviewed: {wasReviewed}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            int someValue = int.Parse(Request.Form["someValue"]);

            if (handler == "handler1")
            {
                // Handle the book loan
                var result = await LoanBookAsync(someValue);
                if (result != null)
                {
                    return Page();
                }
            }
            else if (handler == "addReview")
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return Page();
                }

                if (wasReviewed)
                {
                    ModelState.AddModelError("", "You have already reviewed this book.");
                    return Page();
                }

                NewReview.ReviewDate = DateTime.Now;
                var addedReview = await _reviewService.AddReviewAsync(NewReview);
                if (addedReview == null)
                {
                    ModelState.AddModelError("", "Failed to add review.");
                    return Page();
                }

                wasReviewed = true;
                Book = await _bookService.GetBookByIdAsync(someValue);
                return Page();
            }

            Book = await GetBookDetailsAsync(someValue);
            if (Book == null)
            {
                return NotFound();
            }

            return Page();
        }

        private async Task<Book> LoanBookAsync(int bookId)
        {
            var userOfGG = await _authService.GetCurrentUserAsync();
            var user = await _userService.GetUserByEmail(userOfGG.Email);

            if (user == null || user.UserId <= 0)
            {
                return null;
            }

            return await _bookService.LoanBookAsync(bookId, user.UserId);
        }

        private async Task<Book?> GetBookDetailsAsync(int bookId)
        {
            return await _bookService.GetBookByIdAsync(bookId);
        }
    }
}