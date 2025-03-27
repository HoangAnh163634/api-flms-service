using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

        [HttpGet("config")]
        public IActionResult GetLoanSettings()
        {
            return Ok(_loanSettings);
        }

        [HttpPost("pay")]
        public IActionResult CreatePayment(int loanId, string clientip)
        {
            try
            {
                if (loanId <= 0)
                    return BadRequest("Invalid Loan ID");

                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, loanId, clientip);
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

            // Log toàn bộ dữ liệu nhận được từ VNPAY
            foreach (var param in queryParams)
            {
                Console.WriteLine($"{param.Key}: {param.Value}");
            }

            var transactionId = queryParams["vnp_TxnRef"];
            var responseCode = queryParams["vnp_ResponseCode"];

            // Validate mandatory parameters
            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(responseCode))
            {
                return BadRequest("Missing transaction data");
            }

            // Validate response signature
            var validationResult =  _vnPayService.ValidateResponse(queryParams);
            if (!validationResult.Success)
            {
                return BadRequest(validationResult.Message);
            }

            // Process the payment based on response code
            if (responseCode == "00") // Thanh toán thành công
            {
                await _loanService.MarkAsPaidAsync(int.Parse(transactionId));
                return Ok("Payment successful");
            }

            return BadRequest($"Payment failed: {validationResult.Message}");
        }

    }
}
