namespace api_flms_service.Model
{
    public class LoanRespondDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long OrderId { get; set; }
        public long VnpayTransactionId { get; set; }
        public long Amount { get; set; }
        public string TerminalId { get; set; }
        public string BankCode { get; set; }
    }

}
