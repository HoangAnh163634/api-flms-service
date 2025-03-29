using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _dbContext;

        public ReviewService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _dbContext.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _dbContext.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            // Kiểm tra dữ liệu đầu vào
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null.");
            }

            if (review.BookId <= 0 || review.UserId <= 0)
            {
                throw new ArgumentException("BookId and UserId must be greater than 0.");
            }

            // Kiểm tra xem BookId và UserId có tồn tại trong cơ sở dữ liệu không
            var bookExists = await _dbContext.Books.AnyAsync(b => b.BookId == review.BookId);
            if (!bookExists)
            {
                throw new ArgumentException($"Book with ID {review.BookId} does not exist.");
            }

            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == review.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"User with ID {review.UserId} does not exist.");
            }

            // Log dữ liệu review trước khi thêm
            Console.WriteLine($"Adding review: BookId={review.BookId}, UserId={review.UserId}, Rating={review.Rating}, ReviewText={review.ReviewText}, ReviewDate={review.ReviewDate}");

            try
            {
                // Thêm review vào DbContext
                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync();

                // Log thành công
                Console.WriteLine($"Review added successfully: ReviewId={review.ReviewId}");
                return review;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.WriteLine($"Error adding review: {ex.Message}");
                throw new Exception("Failed to add review to the database.", ex);
            }
        }

        public async Task<Review?> UpdateReviewAsync(Review review)
        {
            var existingReview = await _dbContext.Reviews
                .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
            if (existingReview == null) return null;

            existingReview.Rating = review.Rating;
            existingReview.ReviewText = review.ReviewText;
            existingReview.ReviewDate = review.ReviewDate;
            existingReview.BookId = review.BookId;
            existingReview.UserId = review.UserId;

            try
            {
                _dbContext.Reviews.Update(existingReview);
                await _dbContext.SaveChangesAsync();
                return existingReview;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                throw new Exception("Failed to update review in the database.", ex);
            }
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _dbContext.Reviews.FindAsync(id);
            if (review == null) return false;

            try
            {
                _dbContext.Reviews.Remove(review);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting review: {ex.Message}");
                throw new Exception("Failed to delete review from the database.", ex);
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _dbContext.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}