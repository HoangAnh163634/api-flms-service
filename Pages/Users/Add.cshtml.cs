using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Users
{
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
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            // Thêm người dùng mới
            var addedUser = await _userService.AddUserAsync(User);
            if (addedUser == null)
            {
                ModelState.AddModelError("", "Failed to add user.");
                return Page();
            }

            return RedirectToPage("/Users/Index"); // Quay lại danh sách sau khi thêm
        }
    }
}