using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace api_flms_service.Pages.Loans
{
    [AuthorizeAdmin] // Chỉ cho phép Admin truy cập
    public class EditModel : PageModel
    {
        private readonly ILoanService _loanService;

        public EditModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [BindProperty]
        public Loan Loan { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Loan = await _loanService.GetLoanByIdAsync(id);
            if (Loan == null)
            {
                TempData["ErrorMessage"] = "Loan not found.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Nếu ModelState không hợp lệ, cần lấy lại thông tin Book và User để hiển thị
                var loanWithDetails = await _loanService.GetLoanByIdAsync(Loan.BookLoanId);
                if (loanWithDetails != null)
                {
                    Loan.Book = loanWithDetails.Book;
                    Loan.User = loanWithDetails.User;
                }
                return Page();
            }

            var existingLoan = await _loanService.GetLoanByIdAsync(Loan.BookLoanId);
            if (existingLoan == null)
            {
                TempData["ErrorMessage"] = "Loan not found.";
                // Lấy lại thông tin Book và User để hiển thị trên form
                var loanWithDetails = await _loanService.GetLoanByIdAsync(Loan.BookLoanId);
                if (loanWithDetails != null)
                {
                    Loan.Book = loanWithDetails.Book;
                    Loan.User = loanWithDetails.User;
                }
                return Page();
            }

            // Chỉ cập nhật LoanDate và ReturnDate
            existingLoan.LoanDate = Loan.LoanDate;
            existingLoan.ReturnDate = Loan.ReturnDate;

            try
            {
                await _loanService.UpdateLoanAsync(existingLoan);
                TempData["SuccessMessage"] = "Gia hạn thành công!"; // Sửa thông điệp thành công
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating loan: {ex.Message}";
                // Lấy lại thông tin Book và User để hiển thị trên form
                Loan.Book = existingLoan.Book;
                Loan.User = existingLoan.User;
                return Page();
            }
        }
    }
}