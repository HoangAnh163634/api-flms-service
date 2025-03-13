using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public List<User> Users { get; set; } = new List<User>(); // Thay Model.User bằng Users

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync() // Sử dụng async để lấy dữ liệu
        {
            Users = (await _userService.GetAllUsersAsync()).ToList(); // Lấy danh sách từ service
            Console.WriteLine($"Total users loaded: {Users.Count}");
            return Page();
        }
    }
}