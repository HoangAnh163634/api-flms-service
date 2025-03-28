using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Pages.Reserves.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;

        public IndexModel(IBookService bookService, IUserService userService, AuthService authService, AppDbContext dbContext)
        {
            _bookService = bookService;
            _userService = userService;
            _authService = authService;
            _dbContext = dbContext;
        }

        public List<Loan> Reservations { get; set; } = new List<Loan>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail == null || !(await _userService.IsAuthenticatedAdmin(userEmail)))
            {
                return RedirectToPage("/Account/Login");
            }

            Reservations = await _dbContext.Loans
                .Where(l => l.LoanDate == DateTime.MaxValue && l.ReturnDate == null)
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            if (handler == "delete")
            {
                int reservationId = int.Parse(Request.Form["reservationId"]);
                var reservation = await _dbContext.Loans.FindAsync(reservationId);
                if (reservation != null)
                {
                    _dbContext.Loans.Remove(reservation);
                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToPage();
            }
            return Page();
        }
    }
}