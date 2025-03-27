using api_flms_service.Entity;

namespace api_flms_service.ServiceInterface
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<Loan> CreateLoanAsync(Loan loan);
        Task<bool> ReturnLoanAsync(int id);
        Task<bool> DeleteLoanAsync(int id);
        Task<bool> MarkAsPaidAsync(int loanId);
        Task<bool> UpdateLoanAsync(Loan loan);

    }
}
