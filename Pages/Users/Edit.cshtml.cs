using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public User User { get; set; } = new User();

        [BindProperty]
        public List<Loan> BookLoans { get; set; } = new List<Loan>();

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _userService.GetUserByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            BookLoans = User.BookLoans?.ToList() ?? new List<Loan>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            // Log dữ liệu nhận được để debug
            Console.WriteLine($"User: {User.UserId}, Name: {User.Name}, Email: {User.Email}, Phone: {User.PhoneNumber}");
            Console.WriteLine($"BookLoans Count: {BookLoans?.Count ?? 0}");
            foreach (var loan in BookLoans ?? new List<Loan>())
            {
                Console.WriteLine($"Loan ID: {loan.BookLoanId}, Loan Date: {loan.LoanDate}, Return Date: {loan.ReturnDate}");
            }

            // Cập nhật thông tin User
            var updatedUser = await _userService.UpdateUserAsync(User);
            if (updatedUser == null)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return Page();
            }

            // Cập nhật BookLoans
            if (BookLoans != null && BookLoans.Any())
            {
                foreach (var loan in BookLoans)
                {
                    var existingLoan = await _userService.GetLoanByIdAsync(loan.BookLoanId);
                    if (existingLoan != null)
                    {
                        existingLoan.LoanDate = loan.LoanDate;
                        existingLoan.ReturnDate = loan.ReturnDate;
                        var updatedLoan = await _userService.UpdateLoanAsync(existingLoan);
                        if (updatedLoan == null)
                        {
                            ModelState.AddModelError("", $"Failed to update loan with ID {loan.BookLoanId}.");
                            return Page();
                        }
                    }
                }
            }
            return RedirectToPage("Index");
        }
    }
}
