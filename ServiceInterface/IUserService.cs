using api_flms_service.Model;
using System.Security.Claims;

namespace api_flms_service.ServiceInterface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        int GetCurrentUserId(ClaimsPrincipal user); // Lấy UserId từ JWT hoặc Session
    }
}
