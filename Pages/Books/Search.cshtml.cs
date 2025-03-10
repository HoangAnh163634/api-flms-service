//using api_flms_service.Model;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Text.Json;

//namespace api_flms_service.Pages.Books
//{
//    // Pages/Books/Search.cshtml.cs
//    public class SearchModel : PageModel
//    {
//        private readonly IHttpClientFactory _clientFactory;
//        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        };

//        public SearchModel(IHttpClientFactory clientFactory)
//        {
//            _clientFactory = clientFactory;
//        }

//        public IEnumerable<Book> Books { get; set; } = new List<Book>();
//        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

//        // Keep the original parameter types to avoid method signature changes
//        public async Task OnGetAsync(string? bookName, string? authorName, int? categoryId, int? minPrice, int? maxPrice)
//        {
//            var client = _clientFactory.CreateClient();

//            try
//            {
//                // Lấy danh sách Categories từ API
//                var categoryResponse = await client.GetAsync("http://localhost:5000/api/v0/category");
//                if (categoryResponse.IsSuccessStatusCode)
//                {
//                    var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
//                    Categories = JsonSerializer.Deserialize<List<Category>>(categoryContent, _jsonOptions) ?? new List<Category>();
//                }

//                // Xây dựng query string cho API Books
//                var queryParams = new List<string>();
//                if (!string.IsNullOrEmpty(bookName))
//                    queryParams.Add($"bookName={Uri.EscapeDataString(bookName)}");
//                if (!string.IsNullOrEmpty(authorName))
//                    queryParams.Add($"authorName={Uri.EscapeDataString(authorName)}");
//                if (categoryId.HasValue)
//                    queryParams.Add($"categoryId={categoryId}");
//                if (minPrice.HasValue)
//                    queryParams.Add($"minPrice={minPrice}");
//                if (maxPrice.HasValue)
//                    queryParams.Add($"maxPrice={maxPrice}");

//                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;

//                // Lấy danh sách Books từ API
//                var bookResponse = await client.GetAsync($"http://localhost:5000/api/v0/books/search{queryString}");
//                if (bookResponse.IsSuccessStatusCode)
//                {
//                    var bookContent = await bookResponse.Content.ReadAsStringAsync();
//                    Books = JsonSerializer.Deserialize<List<Book>>(bookContent, _jsonOptions) ?? new List<Book>();
//                }
//            }
//            catch (Exception ex)
//            {
//                // Xử lý exception
//                Books = new List<Book>();
//                Categories = new List<Category>();
//                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
//            }
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Books
{
    public class SearchModel : PageModel
    {
        public void OnGet()
        {
            // Không cần xử lý gì trong hàm này, chỉ trả về trang cho người dùng
        }
    }
}
