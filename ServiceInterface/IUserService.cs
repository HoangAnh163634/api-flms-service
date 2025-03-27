using api_flms_service.Entity;

namespace api_flms_service.ServiceInterface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User> GetUserByEmail(string email, HttpClient client = null, string userImage = null); // Lấy UserId từ JWT hoặc Session
        Task<bool> IsUserAllowedAsync(string email);
        Task<bool> IsAuthenticatedUser(string email);
        Task<bool> IsAuthenticatedAdmin(string email);
        Task<bool> IsAuthenticatedLibrarian(string email);

        // Thêm các phương thức mới cho Loan
        Task<Loan?> GetLoanByIdAsync(int loanId);
        Task<Loan?> UpdateLoanAsync(Loan loan);
    }
}
