using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;
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
            return await _dbContext.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _dbContext.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _dbContext.BookReviews.Add(review);
            await _dbContext.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> UpdateReviewAsync(Review review)
        {
            var existingReview = await _dbContext.BookReviews
                .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
            if (existingReview == null) return null;

            existingReview.Rating = review.Rating;
            existingReview.ReviewText = review.ReviewText;
            existingReview.ReviewDate = review.ReviewDate;
            existingReview.BookId = review.BookId;
            existingReview.UserId = review.UserId;

            _dbContext.BookReviews.Update(existingReview);
            await _dbContext.SaveChangesAsync();
            return existingReview;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _dbContext.BookReviews.FindAsync(id);
            if (review == null) return false;

            _dbContext.BookReviews.Remove(review);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _dbContext.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}