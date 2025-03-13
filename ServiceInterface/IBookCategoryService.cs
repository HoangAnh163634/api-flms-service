using api_flms_service.Entity;

namespace api_flms_service.ServiceInterface
{
    public interface IBookCategoryService
    {
        Task<IEnumerable<BookCategory>> GetAllBooksCategoriesAsync();
        Task<BookCategory?> GetBookCategoryByIdAsync(int id);
        Task<BookCategory> AddBookCategoryAsync(BookCategory BookCategory);
        Task<BookCategory?> UpdateBookCategoryAsync(BookCategory BookCategory);
        Task<bool> DeleteBookCategoryAsync(int id);
    }
}
