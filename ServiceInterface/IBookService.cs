using api_flms_service.Entity;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.ServiceInterface
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<List<Book>> GetBorrowedBooksAsync(int userId);
        Task<IActionResult> RenewBookAsync(int userId, int bookId);
        Task<IEnumerable<Book>> SearchBooksAsync(string? bookName, string? authorName, int? categoryId, int? minPrice, int? maxPrice);
        Task<Book> LoanBookAsync(int bookId, int userId);
    }
}
