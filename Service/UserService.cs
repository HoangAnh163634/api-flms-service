using api_flms_service.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace api_flms_service.ServiceInterface
{

    namespace api_flms_service.Services
    {
        public class UserService : IUserService
        {
            private readonly AppDbContext _dbContext;

            public UserService(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IEnumerable<User>> GetAllUsersAsync()
            {
                return await _dbContext.Users.ToListAsync();
            }

            public async Task<User?> GetUserByIdAsync(int id)
            {
                return await _dbContext.Users.FindAsync(id);
            }

            public async Task<User> AddUserAsync(User user)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }

            public async Task<User?> UpdateUserAsync(User user)
            {
                var existingUser = await _dbContext.Users.FindAsync(user.Id);
                if (existingUser == null) return null;

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Mobile = user.Mobile;
                existingUser.Address = user.Address;

                await _dbContext.SaveChangesAsync();
                return existingUser;
            }

            public async Task<bool> DeleteUserAsync(int id)
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null) return false;

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            public int GetCurrentUserId(ClaimsPrincipal user)
            {
                //var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //return userIdClaim != null ? int.Parse(userIdClaim) : 0;
                return 1;
            }
        }
    }

}
