using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_flms_service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAuthorService _author;
        private readonly IBookService _book;
        private readonly ICategoryService _category;
        private readonly AuthService _auth;
        public CurrentUser? CurrentUser { get; private set; }
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string _BaseUrl { get; set; }
        public List<Category> Categories { get; set; }

        public IndexModel(IAuthorService author, IBookService book, AuthService authService, ICategoryService category, IConfiguration configuration)
        {
            _auth = authService;
            _BaseUrl = configuration["ApiBaseUrl"];
            _author = author;
            _book = book;
            _category = category;
        }

        [BindProperty]
        public List<Entity.Author> authors { get; set; }
        [BindProperty]
        public List<Book> books { get; set; }
        public List<Book> PagedBooks { get; set; } // Danh sách sách cho trang hiện tại
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 3; // 3 sách mỗi trang

        public async Task<IActionResult> OnGetAsync()
        {
            // Log để kiểm tra toàn bộ query string và URL
            Console.WriteLine($"Request URL: {Request.GetEncodedUrl()}");
            Console.WriteLine($"Query String: {Request.QueryString}");

            // Log tất cả tham số trong Request.Query
            Console.WriteLine("All Query Parameters:");
            foreach (var key in Request.Query.Keys)
            {
                Console.WriteLine($"Key: {key}, Value: {Request.Query[key]}");
            }

            // Lấy tham số page, searchTerm, và categoryName thủ công từ Request.Query
            int? page = null;
            string searchTerm = null;
            string categoryName = null;

            if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int parsedPage))
            {
                page = parsedPage;
                Console.WriteLine($"Parsed page from Request.Query: {page}");
            }

            if (Request.Query.ContainsKey("searchTerm"))
            {
                searchTerm = Request.Query["searchTerm"];
                Console.WriteLine($"Parsed searchTerm from Request.Query: {searchTerm}");
            }

            if (Request.Query.ContainsKey("categoryName"))
            {
                categoryName = Request.Query["categoryName"];
                Console.WriteLine($"Parsed categoryName from Request.Query: {categoryName}");
            }

            // Lấy tất cả danh mục
            Categories = (await _category.GetAllCategoriesAsync()).ToList();

            // Lấy tất cả tác giả
            authors = await _author.GetAllAuthorsAsync();

            // Kiểm tra xem có tham số tìm kiếm hay không
            if (!string.IsNullOrEmpty(searchTerm) || !string.IsNullOrEmpty(categoryName))
            {
                // Sử dụng phương thức tìm kiếm nếu có tham số tìm kiếm
                books = (await _book.SearchBooksAsync(searchTerm, categoryName)).ToList();
            }
            else
            {
                // Nếu không có tham số tìm kiếm, lấy tất cả sách
                books = (await _book.GetAllBooksAsync()).ToList();
            }

            // Log để kiểm tra danh sách sách trước khi phân trang
            Console.WriteLine($"Total books before paging: {books.Count}");

            // Phân trang
            CurrentPage = page ?? 1; // Trang hiện tại, mặc định là 1 nếu không có tham số page
            TotalPages = (int)Math.Ceiling(books.Count / (double)PageSize); // Tính tổng số trang
            PagedBooks = books.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList(); // Lấy sách cho trang hiện tại

            // Log để kiểm tra phân trang
            Console.WriteLine($"CurrentPage: {CurrentPage}, TotalPages: {TotalPages}, PagedBooks Count: {PagedBooks.Count}");

            // Xử lý đăng nhập/đăng xuất
            var token = Request?.Query["token"];
            _auth.HandleLogin(Request, Response, token);
            CurrentUser = await _auth.GetCurrentUserAsync();
            LoginUrl = await _auth.GetLoginUrl(Request.GetEncodedUrl());
            LogoutUrl = await _auth.GetLogoutUrl(Request.GetEncodedUrl());
            if (!string.IsNullOrEmpty(token))
            {
                return Redirect(_BaseUrl);
            }
            return Page();
        }
    }
}