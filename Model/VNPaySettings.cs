using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Model
{
    public class VNPaySettings
    {
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string Url { get; set; }
        public string ReturnUrl { get; set; }
    }
}
