using api_flms_service.Entity;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.ServiceInterface
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book, List<IFormFile> images);
        Task<Book> UpdateBookAsync(Book book, List<IFormFile> images);
        Task DeleteBookAsync(int id);
        Task<List<Book>> GetBorrowedBooksAsync(int userId);
        Task<IActionResult> RenewBookAsync(int userId, int bookId); 
        Task<List<Book>> SearchBooks(string bookName, string authorName, string categoryId);

        Task<Book> LoanBookAsync(int bookId, int userId);
    }
}
