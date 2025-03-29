using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using api_flms_service.Model;
using api_flms_service.Entity;
using api_auth_service.Services;
using api_flms_service.ServiceInterface;

namespace api_flms_service.Pages.BookLoans
{
    public class ListModel : PageModel
    {
        private readonly AuthService _auth;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        public readonly ILoanService _loanService;

        public ListModel(AuthService auth, IUserService userService, IReviewService reviewService, ILoanService loanService)
        {
            _auth = auth;
            _userService = userService;
            _reviewService = reviewService;
            _loanService = loanService;
        }

        public List<Loan> BookLoan { get; set; } = default!;
        [BindProperty]
        public List<Loan> BookLoanPast { get; set; } = default!;
        public List<Loan> BookLoanCurrent { get; set; } = default!;
        public Author author { get; set; }
        [BindProperty]
        public string opinion { get; set; }
        [BindProperty]
        public string rating { get; set; }
        [BindProperty]
        public List<Review> BookReviews { get; set; } = default!;
        [BindProperty]
        public User user { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var currentUser = await _auth.GetCurrentUserAsync();
            user = await _userService.GetUserByEmail(currentUser.Email);

            BookReviews = (await _reviewService.GetAllReviewsAsync()).ToList();

            // Log tất cả giá trị LoanDate để kiểm tra
            if (user.BookLoans != null)
            {
                foreach (var loan in user.BookLoans)
                {
                    Console.WriteLine($"Loan ID: {loan.BookLoanId}, LoanDate: {loan.LoanDate}, ReturnDate: {loan.ReturnDate}");
                }
            }

            // Lọc bỏ các bản ghi có LoanDate trong năm 9999 (gần DateTime.MaxValue)
            BookLoan = user.BookLoans?.Where(l => l.LoanDate.HasValue && l.LoanDate.Value.Year < 9999).ToList() ?? new List<Loan>();
            BookLoanCurrent = new List<Loan>();
            BookLoanPast = new List<Loan>();

            foreach (var item in BookLoan)
            {
                if (item.ReturnDate == null || item.ReturnDate > DateTime.UtcNow)
                {
                    BookLoanCurrent.Add(item);
                }
                else
                {
                    BookLoanPast.Add(item);
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(int id, string handler)
        {
            BookReviews = (await _reviewService.GetAllReviewsAsync()).ToList();

            var bookloan = await _loanService.GetLoanByIdAsync(id);

            if (handler == "handler")
            {
                var returned = await _loanService.ReturnLoanAsync(id);
                if (!returned)
                {
                    return Redirect($"/Loans/VNPay?id={id}");
                }
                TempData["SuccessMessage"] = "Book returned successfully!";
            }

            var currentUser = await _auth.GetCurrentUserAsync();
            user = await _userService.GetUserByEmail(currentUser.Email);

            BookReviews = (await _reviewService.GetAllReviewsAsync()).ToList();

            // Log tất cả giá trị LoanDate để kiểm tra
            if (user.BookLoans != null)
            {
                foreach (var loan in user.BookLoans)
                {
                    Console.WriteLine($"Loan ID: {loan.BookLoanId}, LoanDate: {loan.LoanDate}, ReturnDate: {loan.ReturnDate}");
                }
            }

            // Lọc bỏ các bản ghi có LoanDate trong năm 9999 (gần DateTime.MaxValue)
            BookLoan = user.BookLoans?.Where(l => l.LoanDate.HasValue && l.LoanDate.Value.Year < 9999).ToList() ?? new List<Loan>();
            BookLoanCurrent = new List<Loan>();
            BookLoanPast = new List<Loan>();

            foreach (var item in BookLoan)
            {
                if (item.ReturnDate == null || item.ReturnDate > DateTime.UtcNow)
                {
                    BookLoanCurrent.Add(item);
                }
                else
                {
                    BookLoanPast.Add(item);
                }
            }

            return Page();
        }
    }
}