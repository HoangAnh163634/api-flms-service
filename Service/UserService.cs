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
                return await _dbContext.Users
                    .Include(u => u.BookLoans)
                    .ThenInclude(l => l.Book)
                    .ToListAsync();
            }

            public async Task<User?> GetUserByIdAsync(int id)
            {
                // Bao gồm BookLoans khi lấy User
                return await _dbContext.Users
                    .Include(u => u.BookLoans)
                    .ThenInclude(l => l.Book) // Nếu cần thông tin Book
                    .FirstOrDefaultAsync(u => u.UserId == id);
            }

            public async Task<User> AddUserAsync(User user)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }

            public async Task<User?> UpdateUserAsync(User user)
            {
                var existingUser = await _dbContext.Users
                .Include(u => u.BookLoans)
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (existingUser == null) return null;

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Role = user.Role;

                _dbContext.Users.Update(existingUser);
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
                Console.WriteLine($"GetUserByEmail called with email: {email}");

                var user = await _dbContext.Users
                    .Include(e => e.BookLoans)
                        .ThenInclude(e => e.Book)
                            .ThenInclude(b => b.Reviews)
                    .Include(e => e.BookLoans)
                        .ThenInclude(e => e.Book)
                            .ThenInclude(e => e.Author)
                    .Include(e => e.BookReviews)
                        .ThenInclude(r => r.Book)
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    Console.WriteLine($"User with email {email} not found. Creating new user...");
                    user = new User
                    {
                        Email = email,
                        Name = email.Split('@')[0] ?? "Unknown",
                        PhoneNumber = "",
                        Role = "User",
                        RegistrationDate = DateTime.Now
                    };
                    _dbContext.Users.Add(user);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"User created: {email}, Role: {user.Role}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating user: {ex.Message}");
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine($"User found: {user.Email}, UserId: {user.UserId}, Reviews count: {user.BookReviews?.Count ?? 0}");
                    if (user.BookReviews != null)
                    {
                        foreach (var review in user.BookReviews)
                        {
                            Console.WriteLine($"Review: ReviewId={review.ReviewId}, BookId={review.BookId}, UserId={review.UserId}, ReviewText={review.ReviewText}");
                        }
                    }
                }

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

                return user; // Allow access if user or admin exists in the database
            }

            public async Task<bool> IsAuthenticatedUser(string email)
            {
                return await _dbContext.Users.AnyAsync(u => u.Email == email);
            }

            public async Task<bool> IsAuthenticatedAdmin(string email)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
                return user?.Role == "Admin";
            }

            public async Task<bool> IsAuthenticatedLibrarian(string email)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
                return user?.Role == "Librarian";
            }

            // Triển khai phương thức mới cho Loan
            public async Task<Loan?> GetLoanByIdAsync(int loanId)
            {
                return await _dbContext.Loans
                    .Include(l => l.Book) // Nếu cần thông tin Book
                    .FirstOrDefaultAsync(l => l.BookLoanId == loanId);
            }

            public async Task<Loan?> UpdateLoanAsync(Loan loan)
            {
                var existingLoan = await _dbContext.Loans.FindAsync(loan.BookLoanId);
                if (existingLoan == null) return null;

                // Chỉ cập nhật các trường được phép chỉnh sửa
                existingLoan.LoanDate = loan.LoanDate;
                existingLoan.ReturnDate = loan.ReturnDate;

                _dbContext.Loans.Update(existingLoan);
                await _dbContext.SaveChangesAsync();
                return existingLoan;
            }

            
        }
    }
}