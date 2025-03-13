using api_flms_service.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.ServiceInterface
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int id);
        Task<Review> AddReviewAsync(Review review);
        Task<Review?> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId); // Lấy reviews của một user cụ thể
    }
}