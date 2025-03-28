using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;

namespace api_flms_service.Pages.Loans
{
    public class VNPayModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IConfiguration _configuration;

        public Loan Loan { get; set; }
        public Book Book { get; set; }
        public decimal OverdueFee { get; set; }

        public VNPayModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get loan details by loanId (id passed in query string)
            Loan = await _loanService.GetLoanByIdAsync(id);

            if (Loan == null)
            {
                return NotFound(); // Return error if loan is not found
            }

            // Fetch associated book details
            Book = Loan.Book;

            // Calculate overdue fee if applicable
            OverdueFee = _loanService.GetLoanCost(Loan) ?? 0;

            return Page();
        }
    }
}
