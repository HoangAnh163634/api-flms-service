using api_flms_service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace api_flms_service.Pages.Books
{
    [AuthorizeUser]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiBaseUrl is not configured in appsettings.json");
        }

        public List<BookDto> Books { get; set; } = new List<BookDto>();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10); // Đặt timeout để tránh chờ quá lâu

            // Tạo danh sách URL API để thử (HTTP và HTTPS dựa trên ApiBaseUrl)
            var apiUrls = new List<string>();

            // Nếu ApiBaseUrl bắt đầu bằng https, thử cả https và http
            if (_apiBaseUrl.StartsWith("https://"))
            {
                apiUrls.Add($"{_apiBaseUrl}/api/v0/books"); // https://flms.nofpt.com/api/v0/books (Production)
                apiUrls.Add($"{_apiBaseUrl.Replace("https://", "http://")}/api/v0/books"); // Thử HTTP: http://flms.nofpt.com/api/v0/books
            }
            // Nếu ApiBaseUrl bắt đầu bằng http, thử cả http và https
            else if (_apiBaseUrl.StartsWith("http://"))
            {
                apiUrls.Add($"{_apiBaseUrl}/api/v0/books"); // http://localhost:8080/api/v0/books (Development)
                apiUrls.Add($"{_apiBaseUrl.Replace("http://", "https://")}/api/v0/books"); // Thử HTTPS: https://localhost:8080/api/v0/books
            }
            else
            {
                // Nếu ApiBaseUrl không hợp lệ, log lỗi và trả về danh sách rỗng
                Console.WriteLine($"Invalid ApiBaseUrl: {_apiBaseUrl}");
                Books = new List<BookDto>();
                TempData["ErrorMessage"] = "Invalid API base URL configuration.";
                return;
            }

            foreach (var apiUrl in apiUrls)
            {
                try
                {
                    Console.WriteLine($"Attempting to fetch books from: {apiUrl}");
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        Books = JsonSerializer.Deserialize<List<BookDto>>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<BookDto>();
                        Console.WriteLine($"Successfully fetched books from: {apiUrl}");
                        return; // Thoát nếu gọi API thành công
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch books from {apiUrl}. Status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error fetching books from {apiUrl}: {ex.Message}");
                }
            }

            // Nếu không gọi được API từ bất kỳ URL nào
            Books = new List<BookDto>();
            TempData["ErrorMessage"] = "Unable to connect to the book service. Please try again later.";
        }
    }
}