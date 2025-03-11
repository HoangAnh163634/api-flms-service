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

namespace api_flms_service.Pages.Books
{
    public class ShowBookModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;

        public ShowBookModel(IBookService bookService, AuthService authService, IUserService userService)
        {
            _authService = authService;
            _bookService = bookService;
            _userService = userService;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public User User { get; set; } = default!;
        public string role { get; set; }
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SomeValue = (int)id;
            var book = await _bookService.GetBookByIdAsync((int) id);

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

                foreach (var bookloan in User.BookLoans)
                {
                    if (bookloan.Book != null) // Check if bookloan.Book is not null
                    {
                        if (bookloan.Book.BookId == id)
                        {
                            wasLoaned = true;
                        }


                        if (bookloan.Book.Reviews != null && bookloan.Book.Reviews.Count() == 1)
                        {
                            wasReviewed = true;
                        }
                    }
                }

            }
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

            // Retrieve the book details
            Book = await GetBookDetailsAsync(someValue);
            if (Book == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Method to loan the book
        private async Task<Book> LoanBookAsync(int bookId)
        {
            // Get current user from the session
            var userOfGG = await _authService.GetCurrentUserAsync();
            var user = await _userService.GetUserByEmail(userOfGG.Email);

            if (user == null || user.UserId <= 0)
            {
                return null;
            }

            // Call service to loan the book
            return await _bookService.LoanBookAsync(bookId, user.UserId);
        }

        // Method to get book details
        private async Task<Book?> GetBookDetailsAsync(int bookId)
        {
            return await _bookService.GetBookByIdAsync(bookId);
        }



    }



}
