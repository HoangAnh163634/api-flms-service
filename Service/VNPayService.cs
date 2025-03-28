using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using api_flms_service.Model;
using api_flms_service.Entity;
using System.Threading.Tasks;
using api_flms_service.ServiceInterface;
using VNPAY_CS_ASPX;

public class VnPayService
{
    private readonly ILoanService _loanService;
    private readonly IConfiguration _configuration;
    private readonly LoanSettings _loanSettings;
    private readonly IUserService _user;

    public VnPayService(IConfiguration config, IOptions<LoanSettings> loanSettings, ILoanService loanService, IUserService user)
    {
        _configuration = config;
        _loanSettings = loanSettings.Value;
        _loanService = loanService;
        _user = user;
    }

    public async Task<string> CreatePaymentUrl(HttpContext httpContext, int loanId, string clientIp)
    {
        string vnp_Returnurl = _configuration["Vnpay:ReturnUrl"];
        string vnp_Url = _configuration["Vnpay:Url"];
        string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
        string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
        var LoanDeferredTime = int.Parse(_configuration["LoanSettings:LoanDeferredTime"]);
        var LoanCostPerDay = decimal.Parse(_configuration["LoanSettings:LoanCostPerDay"]);

        var loan = await _loanService.GetLoanByIdAsync(loanId);
        decimal OverdueFee = 100000;
        if (loan.LoanDate.HasValue && loan.LoanDate.Value.AddDays(LoanDeferredTime) < DateTime.Now)
        {
            var overdueDays = (DateTime.Now - loan.LoanDate.Value.AddDays(LoanDeferredTime)).Days;
            OverdueFee = overdueDays * LoanCostPerDay; // Calculate overdue fee
        }

        //Get payment input
        OrderInfo order = new OrderInfo();
        order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
        order.Amount = (int)decimal.Round(OverdueFee); // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
        order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
        order.CreatedDate = DateTime.Now;

        VnPayLibrary vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        //if (bankcode_Vnpayqr.Checked == true)
        //{
        //   vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
        //}
        //else if (bankcode_Vnbank.Checked == true)
        //{
        //    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
        //}
        //else if (bankcode_Intcard.Checked == true)
        //{
        //    vnpay.AddRequestData("vnp_BankCode", "INTCARD");
        //}

        vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", clientIp);

        //if (locale_Vn.Checked == true)
        //{
           vnpay.AddRequestData("vnp_Locale", "vn");
        //}
        //else if (locale_En.Checked == true)
        //{
        //    vnpay.AddRequestData("vnp_Locale", "en");
        //}
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + loanId);
        vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        //Add Params of 2.1.0 Version
        //Billing

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

        return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
    }

    public LoanRespondDTO ValidateResponse(IQueryCollection query)
    {
        if (query.Count == 0)
        {
            return new LoanRespondDTO
            {
                Success = false,
                Message = "Không có dữ liệu phản hồi từ VNPAY."
            };
        }

        string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
        VnPayLibrary vnpay = new VnPayLibrary();

        foreach (var key in query.Keys)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnpay.AddResponseData(key, query[key]);
            }
        }

        // Lấy thông tin giao dịch
        bool isValidOrderId = long.TryParse(vnpay.GetResponseData("vnp_TxnRef"), out long orderId);
        bool isValidVnpayTranId = long.TryParse(vnpay.GetResponseData("vnp_TransactionNo"), out long vnpayTranId);
        bool isValidAmount = long.TryParse(vnpay.GetResponseData("vnp_Amount"), out long vnp_Amount);

        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        string vnp_SecureHash = query["vnp_SecureHash"];
        string terminalID = query["vnp_TmnCode"];
        string bankCode = query["vnp_BankCode"];

        vnp_Amount /= 100; // VNPAY trả về giá trị nhân 100, cần chia lại

        if (!isValidOrderId || !isValidVnpayTranId || !isValidAmount)
        {
            return new LoanRespondDTO
            {
                Success = false,
                Message = "Dữ liệu phản hồi không hợp lệ."
            };
        }

        // Kiểm tra chữ ký bảo mật
        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        if (!checkSignature)
        {
            return new LoanRespondDTO
            {
                Success = false,
                Message = "Chữ ký bảo mật không hợp lệ."
            };
        }

        // Kiểm tra trạng thái giao dịch
        bool isSuccess = vnp_ResponseCode == "00" && vnp_TransactionStatus == "00";
        string message = isSuccess ? "Giao dịch thành công." : $"Giao dịch thất bại. Mã lỗi: {vnp_ResponseCode}";

        return new LoanRespondDTO
        {
            Success = isSuccess,
            Message = message,
            OrderId = orderId,
            VnpayTransactionId = vnpayTranId,
            Amount = vnp_Amount,
            TerminalId = terminalID,
            BankCode = bankCode
        };
    }


}
