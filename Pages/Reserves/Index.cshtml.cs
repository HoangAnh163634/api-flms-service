using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Pages.Reserves
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;

        public IndexModel(IBookService bookService, AuthService authService, IUserService userService)
        {
            _bookService = bookService;
            _authService = authService;
            _userService = userService;
        }

        public List<Book> ReservedBooks { get; set; } = new List<Book>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = (await _authService.GetCurrentUserAsync())?.Email;
            if (userEmail == null) return RedirectToPage("/Account/Login");

            var user = await _userService.GetUserByEmail(userEmail);
            if (user == null) return RedirectToPage("/Account/Login");

            ReservedBooks = await _bookService.GetReservedBooksAsync(user.UserId);
            return Page();
        }
    }
}