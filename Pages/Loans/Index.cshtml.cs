using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.Pages.Loans
{
    [AuthorizeAdmin] // Chỉ cho phép Admin truy cập
    public class IndexModel : PageModel
    {
        private readonly ILoanService _loanService;

        public IndexModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public IEnumerable<Loan> Loans { get; set; } = new List<Loan>();

        public async Task OnGetAsync()
        {
            Loans = await _loanService.GetAllLoansAsync();
        }
    }
}