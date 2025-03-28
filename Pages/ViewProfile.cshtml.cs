using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace api_flms_service.Pages
{
    public class ViewProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly AuthService _authService;

        public ViewProfileModel(IUserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [BindProperty]
        public User User { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                Console.WriteLine("Current user is null. Redirecting to Index...");
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"Current user email: {currentUser.Email}");
            User = await _userService.GetUserByEmail(currentUser.Email);
            if (User == null)
            {
                Console.WriteLine("User not found after GetUserByEmail. Redirecting to Index...");
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"User loaded: {User.Email}, UserId: {User.UserId}, Reviews count: {User.BookReviews?.Count ?? 0}");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userService.GetUserByEmail(currentUser.Email);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            user.Name = User.Name;
            user.PhoneNumber = User.PhoneNumber;

            try
            {
                // Cập nhật thông tin người dùng (giả định có phương thức UpdateUser trong IUserService)
                await _userService.UpdateUserAsync(user);
                SuccessMessage = "Profile updated successfully!";
            }
            catch
            {
                ErrorMessage = "An error occurred while updating the profile.";
            }

            User = user;
            return Page();
        }
    }
}