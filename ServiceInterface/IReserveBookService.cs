using api_flms_service.Entity;
using api_flms_service.Model;

namespace api_flms_service.ServiceInterface
{
    public interface IReserveBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(string authorFilter = null);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book?> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
    }
}

