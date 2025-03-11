using api_flms_service.Entity;
using api_flms_service.Model;
using Microsoft.EntityFrameworkCore;
namespace api_flms_service.ServiceInterface
{

    namespace api_flms_service.Services
    {
        public class UserService : IUserService
        {
            private readonly AppDbContext _dbContext;
            private readonly IConfiguration _configuration;

            public UserService(AppDbContext dbContext, IConfiguration configuration)
            {
                _dbContext = dbContext;
                _configuration = configuration;
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
                var existingUser = await _dbContext.Users.FindAsync(user.UserId);
                if (existingUser == null) return null;

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

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

            public async Task<User> GetUserByEmail(string email)
            {
               var user = await _dbContext.Users
                                .Include(e => e.BookLoans)
                                .ThenInclude(e => e.Book)
                                .ThenInclude(b => b.Reviews)
                                .FirstOrDefaultAsync(x => x.Email == email);

                return user;
            }

            public async Task<bool> IsUserAllowedAsync(string email)
            {
                var allowedEmails = _configuration.GetSection("AllowEmail").Get<string[]>();

                if (allowedEmails != null && allowedEmails.Any(format => email.EndsWith(format)))
                {
                    return true; // Email matches one of the allowed formats
                }

                // Check the database for an existing user or admin
                var user = await _dbContext.Users.AnyAsync(u => u.Email == email);
                var admin = await _dbContext.Admins.AnyAsync(a => a.Email == email);

                return user || admin; // Allow access if user or admin exists in the database
            }

            public async Task<bool> IsAuthenticatedUser(string email)
            {
                return await _dbContext.Users.AnyAsync(u => u.Email == email);
            }

            public async Task<bool> IsAuthenticatedAdmin(string email)
            {
                return await _dbContext.Admins.AnyAsync(a => a.Email == email);
            }
        }
    }

}
