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
        public readonly ILoanService _loanService;

        public ShowBookModel(IBookService bookService, AuthService authService, IUserService userService, IReviewService reviewService, ILoanService loanService)
        {
            _authService = authService;
            _bookService = bookService;
            _userService = userService;
            _reviewService = reviewService;
            _loanService = loanService;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public User User { get; set; } = default!;
        public string role { get; set; } = "guest";
        public string errorMessage { get; set; } = "";
        [BindProperty]
        public int SomeValue { get; set; }
        [BindProperty]
        public bool wasLoaned { get; set; } = false;
        [BindProperty]
        public bool wasReviewed { get; set; } = false;
        [BindProperty]
        public bool hasReserved { get; set; } = false;

        [BindProperty]
        public Review NewReview { get; set; } = new Review();

        [BindProperty]
        public Review EditReview { get; set; }

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
                await Console.Out.WriteLineAsync($"OnGet - Book ID: {id}, AvailableCopies: {Book.AvailableCopies}");
                await Console.Out.WriteLineAsync($"OnGet - Reviews Count: {Book.Reviews.Count()}");
                await Console.Out.WriteLineAsync($"OnGet - Categories Count: {Book.Categories.Count()}");
            }

            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail != null)
            {
                User = await _userService.GetUserByEmail(userEmail);
                role = User?.Role ?? "guest";

                if (User != null)
                {
                    await Console.Out.WriteLineAsync($"OnGet - User ID: {User.UserId}, Email: {User.Email}, BookLoans Count: {User.BookLoans?.Count ?? 0}");
                    foreach (var bookloan in User.BookLoans)
                    {
                        await Console.Out.WriteLineAsync($"OnGet - BookLoan - BookId: {bookloan.BookId}, LoanDate: {bookloan.LoanDate}, ReturnDate: {bookloan.ReturnDate}, Book: {(bookloan.Book != null ? bookloan.Book.Title : "null")}");
                        if (bookloan.Book != null && bookloan.Book.BookId == id)
                        {
                            var maxValueThreshold = DateTime.MaxValue.AddDays(-1);
                            if (bookloan.LoanDate >= maxValueThreshold && bookloan.ReturnDate == null)
                            {
                                hasReserved = true;
                                await Console.Out.WriteLineAsync($"OnGet - Book {id} is reserved by user {User.UserId}");
                            }
                            else if (bookloan.LoanDate < maxValueThreshold && (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow))
                            {
                                wasLoaned = true;
                                await Console.Out.WriteLineAsync($"OnGet - Book {id} is loaned by user {User.UserId}, ReturnDate: {bookloan.ReturnDate}");
                            }
                        }
                    }

                    if (Book.Reviews != null && Book.Reviews.Any(r => r.UserId == User.UserId))
                    {
                        wasReviewed = true;
                        await Console.Out.WriteLineAsync($"OnGet - User {User.UserId} has reviewed Book {id}");
                    }
                }
            }

            NewReview.BookId = (int)id;
            if (User != null) NewReview.UserId = User.UserId;

            await Console.Out.WriteLineAsync($"OnGet - Role: {role}, WasLoaned: {wasLoaned}, WasReviewed: {wasReviewed}, HasReserved: {hasReserved}");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            int someValue = int.Parse(Request.Form["someValue"]);
            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail != null)
            {
                User = await _userService.GetUserByEmail(userEmail);
                role = User?.Role ?? "guest";
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("", "You must be logged in to perform this action.");
                Book = await _bookService.GetBookByIdAsync(someValue);
                return Page();
            }

            if (handler == "handler1")
            {
                ModelState.Clear();
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
            else if (handler == "reserve")
            {
                ModelState.Clear();
                var result = await _bookService.ReserveBookAsync(someValue, User.UserId);
                if (result.Contains("successfully"))
                {
                    TempData["SuccessMessage"] = "Book reserved successfully!";
                    return RedirectToPage(new { id = someValue });
                }
                else
                {
                    ModelState.AddModelError("", result);
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }
            }
            else if (handler == "addReview")
            {
                ModelState.Clear();
                TryValidateModel(NewReview, nameof(NewReview));

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    Book = await _bookService.GetBookByIdAsync(someValue);

                    if (User != null)
                    {
                        var maxValueThreshold = DateTime.MaxValue.AddDays(-1);
                        foreach (var bookloan in User.BookLoans)
                        {
                            if (bookloan.Book != null && bookloan.Book.BookId == someValue)
                            {
                                if (bookloan.LoanDate >= maxValueThreshold && bookloan.ReturnDate == null)
                                {
                                    hasReserved = true;
                                }
                                else if (bookloan.LoanDate < maxValueThreshold && (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow))
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

                    await Console.Out.WriteLineAsync($"Validation failed - Role: {role}, WasLoaned: {wasLoaned}, WasReviewed: {wasReviewed}, HasReserved: {hasReserved}");
                    return Page();
                }

                if (role != "User")
                {
                    ModelState.AddModelError("", "Only users with role 'User' can add reviews.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                if (!wasLoaned)
                {
                    ModelState.AddModelError("", "You must loan the book to add a review.");
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
            else if (handler == "editReview")
            {
                ModelState.Clear();
                int reviewId = int.Parse(Request.Form["reviewId"]);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                if (review == null)
                {
                    ModelState.AddModelError("", "Review not found.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                if (review.UserId != User.UserId)
                {
                    ModelState.AddModelError("", "You can only edit your own reviews.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                EditReview = review;
                Book = await _bookService.GetBookByIdAsync(someValue);

                if (User != null)
                {
                    var maxValueThreshold = DateTime.MaxValue.AddDays(-1);
                    foreach (var bookloan in User.BookLoans)
                    {
                        if (bookloan.Book != null && bookloan.Book.BookId == someValue)
                        {
                            if (bookloan.LoanDate >= maxValueThreshold && bookloan.ReturnDate == null)
                            {
                                hasReserved = true;
                            }
                            else if (bookloan.LoanDate < maxValueThreshold && (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow))
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

                return Page();
            }
            else if (handler == "updateReview")
            {
                ModelState.Clear();
                TryValidateModel(EditReview, nameof(EditReview));

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    Book = await _bookService.GetBookByIdAsync(someValue);

                    if (User != null)
                    {
                        var maxValueThreshold = DateTime.MaxValue.AddDays(-1);
                        foreach (var bookloan in User.BookLoans)
                        {
                            if (bookloan.Book != null && bookloan.Book.BookId == someValue)
                            {
                                if (bookloan.LoanDate >= maxValueThreshold && bookloan.ReturnDate == null)
                                {
                                    hasReserved = true;
                                }
                                else if (bookloan.LoanDate < maxValueThreshold && (bookloan.ReturnDate == null || bookloan.ReturnDate > DateTime.UtcNow))
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

                    return Page();
                }

                if (role != "User")
                {
                    ModelState.AddModelError("", "Only users with role 'User' can edit reviews.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                var existingReview = await _reviewService.GetReviewByIdAsync(EditReview.ReviewId);
                if (existingReview == null)
                {
                    ModelState.AddModelError("", "Review not found.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                if (existingReview.UserId != User.UserId)
                {
                    ModelState.AddModelError("", "You can only edit your own reviews.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                EditReview.ReviewDate = DateTime.Now;
                var updatedReview = await _reviewService.UpdateReviewAsync(EditReview);
                if (updatedReview == null)
                {
                    ModelState.AddModelError("", "Failed to update review.");
                    Book = await _bookService.GetBookByIdAsync(someValue);
                    return Page();
                }

                TempData["SuccessMessage"] = "Review updated successfully!";
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