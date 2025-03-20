using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace api_flms_service.Pages.Loans
{
    public class PaymentCallbackModel : PageModel
    {
        private const string HashSecret = "S3cr3tK3yF0rVNPayT3st";

        public bool PaymentSuccess { get; set; }
        public string TransactionId { get; set; }

        public void OnGet()
        {
            var queryParams = Request.Query;
            var responseCode = queryParams["vnp_ResponseCode"];
            var secureHash = queryParams["vnp_SecureHash"];

            if (ValidateResponse(queryParams, secureHash) && responseCode == "00")
            {
                PaymentSuccess = true;
                TransactionId = queryParams["vnp_TxnRef"];
            }
            else
            {
                PaymentSuccess = false;
            }
        }

        private bool ValidateResponse(IQueryCollection queryParams, string receivedHash)
        {
            var vnpayData = queryParams
                .Where(kvp => kvp.Key.StartsWith("vnp_") && kvp.Key != "vnp_SecureHash")
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            string queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            string calculatedHash = CreateSignature(queryString);

            return calculatedHash == receivedHash;
        }

        private string CreateSignature(string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(HashSecret));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
        }
    }
}
