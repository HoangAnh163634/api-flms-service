using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace api_flms_service.Pages.Loans
{
    public class VNPayModel : PageModel
    {
        private const string TmnCode = "ABC12345";  // Example Merchant Code
        private const string HashSecret = "S3cr3tK3yF0rVNPayT3st"; // Example Hash Secret
        private const string VnPayUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private const string ReturnUrl = "https://yourdomain.com/VNPay/PaymentCallback";

        public IActionResult OnPost(int BookLoanId, int Amount)
        {
            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", TmnCode },
                { "vnp_Amount", (Amount * 100).ToString() }, // Convert VND to smallest unit
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", BookLoanId.ToString() },
                { "vnp_OrderInfo", $"Payment for Loan {BookLoanId}" },
                { "vnp_OrderType", "billpayment" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", ReturnUrl },
                { "vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            string queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            string secureHash = CreateSignature(queryString);

            string paymentUrl = $"{VnPayUrl}?{queryString}&vnp_SecureHash={secureHash}";

            return Redirect(paymentUrl);
        }

        private string CreateSignature(string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(HashSecret));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
        }
    }
}
