using api_flms_service.Entity;
using api_flms_service.Model;
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
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm, string categoryName); // Sửa thành BookDto
        Task<Book> LoanBookAsync(int bookId, int userId);
        Task<string> ReserveBookAsync(int bookId, int userId);
        Task<List<Book>> GetReservedBooksAsync(int userId);
        Task<bool> AuthorExistsAsync(int authorId);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}