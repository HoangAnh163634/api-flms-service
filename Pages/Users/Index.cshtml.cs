using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public List<User> Users { get; set; } = new List<User>();

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGet()
        {
            Users = (await _userService.GetAllUsersAsync()).ToList();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToPage();
        }
    }
}
