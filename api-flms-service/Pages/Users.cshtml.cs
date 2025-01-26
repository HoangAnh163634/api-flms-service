using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages
{
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;

        public List<User> Users { get; set; } = new List<User>();

        public UsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGet()
        {
            Users = (await _userService.GetAllUsersAsync()).ToList();
        }
    }
}
