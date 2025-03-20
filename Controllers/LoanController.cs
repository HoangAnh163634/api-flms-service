using api_flms_service.Entity;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Controllers
{
    [Route("api/v0/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        private readonly VNPayService _vnPayService;

        public LoanController(ILoanService loanService, VNPayService vnPayService)
        {
            _loanService = loanService;
            _vnPayService = vnPayService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoan(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoan([FromBody] Loan loan)
        {
            try
            {
                var newLoan = await _loanService.CreateLoanAsync(loan);
                return CreatedAtAction(nameof(GetLoan), new { id = newLoan.BookLoanId }, newLoan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("return/{id}")]
        public async Task<IActionResult> ReturnLoan(int id)
        {
            var success = await _loanService.ReturnLoanAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var success = await _loanService.DeleteLoanAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("pay")]
        public IActionResult CreatePayment([FromBody] Loan loan)
        {
            var paymentUrl = _vnPayService.CreatePaymentUrl(loan, HttpContext);
            return Ok(new { url = paymentUrl });
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var queryParams = Request.Query;
            var transactionId = queryParams["vnp_TxnRef"];
            var responseCode = queryParams["vnp_ResponseCode"];
            var secureHash = queryParams["vnp_SecureHash"];

            if (!_vnPayService.ValidateResponse(queryParams, secureHash))
            {
                return BadRequest("Invalid signature");
            }

            if (responseCode == "00") // Payment successful
            {
                await _loanService.MarkAsPaidAsync(int.Parse(transactionId));
                return Ok("Payment successful");
            }

            return BadRequest("Payment failed");
        }

    }
}
