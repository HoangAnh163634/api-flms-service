using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using api_flms_service.Entity;
using api_flms_service.Model;

public class VNPayService
{
    private readonly VNPaySettings _settings;

    public VNPayService(IOptions<VNPaySettings> settings)
    {
        _settings = settings.Value;
    }

    // ✅ **Generate VNPay Payment URL**
    public string CreatePaymentUrl(Loan loan, HttpContext context)
    {
        var vnpayData = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _settings.TmnCode },
            { "vnp_Amount", (loan.BookId * 100000).ToString() }, // Example: Price based on BookId
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", loan.BookLoanId.ToString() },
            { "vnp_OrderInfo", $"Payment for Loan {loan.BookLoanId}" },
            { "vnp_OrderType", "billpayment" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", _settings.ReturnUrl },
            { "vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
        };

        string queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
        string secureHash = CreateSignature(queryString);
        return $"{_settings.Url}?{queryString}&vnp_SecureHash={secureHash}";
    }

    // ✅ **Create Signature for Secure Transactions**
    private string CreateSignature(string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HashSecret));
        return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
    }

    // ✅ **Validate VNPay Response Signature**
    public bool ValidateResponse(IQueryCollection queryParams, string receivedHash)
    {
        var vnpayData = queryParams
            .Where(kvp => kvp.Key.StartsWith("vnp_") && kvp.Key != "vnp_SecureHash")
            .OrderBy(kvp => kvp.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

        string queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
        string calculatedHash = CreateSignature(queryString);

        return calculatedHash == receivedHash;
    }
}
