using api_flms_service.Model;

namespace api_flms_service.ServiceInterface
{
    public interface IIssuedBookService
    {
        Task<IEnumerable<IssuedBook>> GetAllIssuedBooksAsync();
        Task<IssuedBook?> GetIssuedBookByIdAsync(int id);
        Task<IssuedBook> AddIssuedBookAsync(IssuedBook issuedBook);
        Task<IssuedBook?> UpdateIssuedBookAsync(IssuedBook issuedBook);
        Task<bool> DeleteIssuedBookAsync(int id);
    }
}
