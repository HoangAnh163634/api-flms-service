using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api_flms_service.Controllers
{
    [Route("api/v0/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        private readonly VnPayService _vnPayService;
        private readonly LoanSettings _loanSettings;

        public LoanController(ILoanService loanService, VnPayService vnPayService, IOptions<LoanSettings> loanSettings)
        {
            _loanService = loanService;
            _vnPayService = vnPayService;
            _loanSettings = loanSettings.Value;
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
            if (loan == null)
                return BadRequest("Loan data is required");

            if (loan.BookLoanId <= 0)
                return BadRequest("Invalid Loan ID");

            try
            {
                var newLoan = await _loanService.CreateLoanAsync(loan);
                return CreatedAtAction(nameof(GetLoan), new { id = newLoan.BookLoanId }, newLoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
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

        [HttpGet("config")]
        public IActionResult GetLoanSettings()
        {
            return Ok(_loanSettings);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> CreatePayment(int loanId, string clientip)
        {
            try
            {
                if (loanId <= 0)
                    return BadRequest("Invalid Loan ID");

                var paymentUrl = await _vnPayService.CreatePaymentUrl(HttpContext, loanId, clientip);
                if (string.IsNullOrEmpty(paymentUrl))
                    return BadRequest("Failed to create payment URL");

                return Ok(new { url = paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var queryParams = Request.Query;

            if (!queryParams.ContainsKey("vnp_TxnRef") || !queryParams.ContainsKey("vnp_ResponseCode"))
            {
                return BadRequest("Missing transaction data");
            }

            var transactionId = queryParams["vnp_TxnRef"].ToString();
            var responseCode = queryParams["vnp_ResponseCode"].ToString();
            var info = queryParams["vnp_OrderInfo"].ToString();

            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(responseCode))
            {
                return BadRequest("Invalid transaction data");
            }

            var validationResult = _vnPayService.ValidateResponse(queryParams);
            if (!validationResult.Success)
            {
                return BadRequest(validationResult.Message);
            }

            if (responseCode == "00") // Thanh toán thành công
            {
                string pattern = @"Thanh toan don hang: (\d+)";
                Match match = Regex.Match(info, pattern);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int lId))
                    {
                        var paid = await _loanService.MarkAsPaidAsync(lId);
                        if (paid)
                        {
                            await _loanService.ReturnLoanAsync(lId);
                        }
                    }
                }
                else
                {
                    return BadRequest("loanId not found.");
                }

                return Ok("Payment successful");
            }

            return BadRequest($"Payment failed: {validationResult.Message}");
        }

    }
}
