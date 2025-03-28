using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.Service;
using Microsoft.EntityFrameworkCore;
using System.Net;
namespace api_flms_service.ServiceInterface
{

    namespace api_flms_service.Services
    {
        public class UserService : IUserService
        {
            private readonly AppDbContext _dbContext;
            private readonly ICloudinaryService _cloudinaryService;
            private readonly IConfiguration _configuration;
            private readonly List<string> _allowTailEmail;

            public UserService(AppDbContext dbContext, IConfiguration configuration, ICloudinaryService cloudinaryService)
            {
                _dbContext = dbContext;
                _configuration = configuration;
                _cloudinaryService = cloudinaryService;
                _allowTailEmail = _configuration.GetSection("AllowEmail").Get<List<string>>() ?? new List<string>();
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

            public async Task<User> GetUserByEmail(string email, HttpClient client = null, string userImage = null)
            {
                Console.WriteLine($"GetUserByEmail called with email: {email}");

                var user = await _dbContext.Users
                    .Include(e => e.BookLoans)
                        .ThenInclude(bl => bl.Book)
                            .ThenInclude(b => b.Reviews)
                    .Include(e => e.BookLoans)
                        .ThenInclude(bl => bl.Book)
                            .ThenInclude(b => b.Author)
                    .Include(e => e.BookReviews)
                        .ThenInclude(br => br.Book)
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    string downloadUrl = null;

                    if (!string.IsNullOrEmpty(userImage) && client != null)
                    {
                        try
                        {
                            var response = await client.GetAsync(userImage);
                            if (response.IsSuccessStatusCode)
                            {
                                await using var stream = await response.Content.ReadAsStreamAsync();
                                using var memoryStream = new MemoryStream();
                                await stream.CopyToAsync(memoryStream);
                                memoryStream.Position = 0;

                                // Chuyển `MemoryStream` thành `IFormFile`
                                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, $"file", $"user_image_{Guid.NewGuid().ToString().Replace('-','_')}.jpg")
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg"
                                };

                                downloadUrl = await _cloudinaryService.UploadFileAsync(formFile);
                            }
                            else
                            {
                                Console.WriteLine($"Failed to download image: {userImage}. Status code: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error downloading user image: {ex.Message}");
                        }
                    }

                    Console.WriteLine($"User with email {email} not found. Creating new user...");
                    var isAllow = _allowTailEmail.Any(e => email.Contains(e));

                    user = new User
                    {
                        Email = email,
                        Name = email.Split('@')[0] ?? "Unknown",
                        PhoneNumber = "",
                        Role = isAllow ? "User" : "Guest",
                        GoogleImage = userImage,
                        LocalImage = downloadUrl,
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
                    var downloadUrl = user.LocalImage;

                    if (!string.IsNullOrEmpty(userImage) && client != null && userImage != user.GoogleImage)
                    {
                        try
                        {
                            var response = await client.GetAsync(userImage);
                            if (response.IsSuccessStatusCode)
                            {
                                await using var stream = await response.Content.ReadAsStreamAsync();
                                using var memoryStream = new MemoryStream();
                                await stream.CopyToAsync(memoryStream);
                                memoryStream.Position = 0;

                                // Chuyển `MemoryStream` thành `IFormFile`
                                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, $"file", $"user_image_{Guid.NewGuid().ToString().Replace('-', '_')}.jpg")
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg"
                                };

                                downloadUrl = await _cloudinaryService.UploadFileAsync(formFile);
                                
                                user.GoogleImage = userImage;
                                user.LocalImage = downloadUrl;
                                _dbContext.Users.Update(user);

                                try
                                {
                                    await _dbContext.SaveChangesAsync();
                                    Console.WriteLine($"User update image: {email}, Role: {user.Role}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error update user: {ex.Message}");
                                    throw;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Failed to download image: {userImage}. Status code: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error downloading user image: {ex.Message}");
                        }
                    }

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