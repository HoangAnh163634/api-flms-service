//using api_auth_service.Services;
//using api_flms_service.Entity;
//using api_flms_service.ServiceInterface;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http.Extensions;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace api_flms_service.Pages
//{
//    public class IndexModel : PageModel
//    {
//        private readonly IAuthorService _author;
//        private readonly IBookService _book;
//        private readonly ICategoryService _category;

//        private readonly AuthService _auth;
//        public CurrentUser? CurrentUser { get; private set; }
//        public string LoginUrl { get; set; }
//        public string LogoutUrl { get; set; }
//        public string _BaseUrl { get; set; }
//        public List<Category> Categories { get; set; }

//        public IndexModel(IAuthorService author, IBookService book, AuthService authService,ICategoryService category, IConfiguration configuration)
//        {
//            _auth = authService;
//            _BaseUrl = configuration["ApiBaseUrl"];
//            _author = author;
//            _book = book;
//            _category = category;

//        }

//        [BindProperty]
//        public List<Entity.Author> authors { get; set; }
//        [BindProperty]
//        public List<Book> books { get; set; }

//        public async Task<IActionResult> OnGetAsync()
//        {   
//            Categories = (await _category.GetAllCategoriesAsync()).ToList();
//            authors = await _author.GetAllAuthorsAsync();
//            books = (await _book.GetAllBooksAsync()).ToList();
//            var token = Request?.Query["token"];

//            _auth.HandleLogin(Request, Response, token);

//            CurrentUser = await _auth.GetCurrentUserAsync();
//            LoginUrl = await _auth.GetLoginUrl(Request.GetEncodedUrl());
//            LogoutUrl = await _auth.GetLogoutUrl(Request.GetEncodedUrl());
//            if (!string.IsNullOrEmpty(token))
//            {
//                return Redirect(_BaseUrl);
//            }
//            return Page();
//        }

//    }
//}


using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<IActionResult> OnGetAsync(string searchTerm, string categoryName, int? publicationYear)
        {
            // Lấy tất cả danh mục
            Categories = (await _category.GetAllCategoriesAsync()).ToList();

            // Lấy tất cả tác giả
            authors = await _author.GetAllAuthorsAsync();

            // Kiểm tra xem có tham số tìm kiếm hay không
            if (!string.IsNullOrEmpty(searchTerm) || !string.IsNullOrEmpty(categoryName) || publicationYear.HasValue)
            {
                // Sử dụng phương thức tìm kiếm nếu có tham số tìm kiếm
                books = (await _book.SearchBooksAsync(searchTerm, categoryName, publicationYear)).ToList();
            }
            else
            {
                // Nếu không có tham số tìm kiếm, lấy tất cả sách
                books = (await _book.GetAllBooksAsync()).ToList();
            }

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

