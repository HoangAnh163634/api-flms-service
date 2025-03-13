using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Service
{
    public class BookBookCategoryService : IBookCategoryService
    {
        private readonly AppDbContext _dbContext;

        public BookBookCategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all BookCategory entities
        public async Task<IEnumerable<BookCategory>> GetAllBooksCategoriesAsync()
        {
            return await _dbContext.BookCategories
                .Include(bc => bc.Book)  // Including the related Book data if needed
                .Include(bc => bc.Category)  // Including the related Category data if needed
                .ToListAsync();
        }

        // Get a BookCategory by ID
        public async Task<BookCategory?> GetBookCategoryByIdAsync(int id)
        {
            return await _dbContext.BookCategories
                .Include(bc => bc.Book)
                .Include(bc => bc.Category)
                .FirstOrDefaultAsync(bc => bc.BookId == id);
        }

        // Add a new BookCategory
        public async Task<BookCategory> AddBookCategoryAsync(BookCategory bookCategory)
        {
            _dbContext.BookCategories.Add(bookCategory);
            await _dbContext.SaveChangesAsync();
            return bookCategory;
        }

        // Update an existing BookCategory
        public async Task<BookCategory?> UpdateBookCategoryAsync(BookCategory bookCategory)
        {
            var existingBookCategory = await _dbContext.BookCategories.FindAsync(bookCategory.BookId, bookCategory.CategoryId);
            if (existingBookCategory == null)
            {
                return null;  // Not found, so we return null
            }

            existingBookCategory.Book = bookCategory.Book;  // Update properties as needed
            existingBookCategory.Category = bookCategory.Category;

            _dbContext.BookCategories.Update(existingBookCategory);
            await _dbContext.SaveChangesAsync();
            return existingBookCategory;
        }

        // Delete a BookCategory by ID
        public async Task<bool> DeleteBookCategoryAsync(int id)
        {
            var bookCategory = await _dbContext.BookCategories.FindAsync(id);
            if (bookCategory == null)
            {
                return false;  // Not found
            }

            _dbContext.BookCategories.Remove(bookCategory);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
