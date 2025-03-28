using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.Pages
{
    [AuthorizeAdmin]
    public class LoansModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;

        public LoansModel(ILoanService loanService, IBookService bookService, IUserService userService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _userService = userService;
        }

        public IList<Loan> Loans { get; set; } = new List<Loan>();
        public bool IsReservedFilter { get; set; } = false; // Biến để xác định xem có đang lọc "reserve" không

        [BindProperty]
        public Loan Loan { get; set; }

        public async Task<IActionResult> OnGetAsync(string filter)
        {
            var allLoans = await _loanService.GetAllLoansAsync();

            // Kiểm tra tham số filter
            if (filter == "reserved")
            {
                IsReservedFilter = true;
                var maxValueThreshold = DateTime.MaxValue.AddDays(-1);
                Loans = allLoans
                    .Where(l => l.LoanDate >= maxValueThreshold && l.ReturnDate == null)
                    .ToList();
            }
            else
            {
                IsReservedFilter = false;
                Loans = allLoans.ToList();
            }

            // Load thông tin Book và User cho từng Loan
            foreach (var loan in Loans)
            {
                loan.Book = await _bookService.GetBookByIdAsync(loan.BookId);
                loan.User = await _userService.GetUserByEmail(loan.User?.Email ?? "");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Loans = (await _loanService.GetAllLoansAsync()).ToList();
                foreach (var loan in Loans)
                {
                    loan.Book = await _bookService.GetBookByIdAsync(loan.BookId);
                    loan.User = await _userService.GetUserByEmail(loan.User?.Email ?? "");
                }
                return Page();
            }

            var result = await _loanService.UpdateLoanAsync(Loan);
            if (result == null)
            {
                ModelState.AddModelError("", "Failed to update loan.");
                Loans = (await _loanService.GetAllLoansAsync()).ToList();
                foreach (var loan in Loans)
                {
                    loan.Book = await _bookService.GetBookByIdAsync(loan.BookId);
                    loan.User = await _userService.GetUserByEmail(loan.User?.Email ?? "");
                }
                return Page();
            }

            TempData["SuccessMessage"] = "Loan updated successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            Loan = loan;
            Loans = (await _loanService.GetAllLoansAsync()).ToList();
            foreach (var l in Loans)
            {
                l.Book = await _bookService.GetBookByIdAsync(l.BookId);
                l.User = await _userService.GetUserByEmail(l.User?.Email ?? "");
            }

            return Page();
        }
    }
}