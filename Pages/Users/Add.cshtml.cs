using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Users
{
    [AuthorizeUser]
    public class AddModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public User User { get; set; } = new User();

        public AddModel(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return Page();
            }

            // Thêm ngày đăng ký mặc định nếu cần
            User.RegistrationDate = DateTime.Now;

            try
            {
                var addedUser = await _userService.AddUserAsync(User);
                if (addedUser == null)
                {
                    ModelState.AddModelError("", "Failed to add user. Please try again.");
                    return Page();
                }
                return RedirectToPage("/Users/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}