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

        private readonly AuthService _auth;
        public CurrentUser? CurrentUser { get; private set; }
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string _BaseUrl { get; set; }

        public IndexModel(IAuthorService author, IBookService book, AuthService authService, IConfiguration configuration)
        {
            _auth = authService;
            _BaseUrl = configuration["ApiBaseUrl"];
            _author = author;
            _book = book;
        }

        [BindProperty]
        public List<Entity.Author> authors { get; set; }
        [BindProperty]
        public List<Book> books { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            authors = await _author.GetAllAuthorsAsync();
            books = (await _book.GetAllBooksAsync()).ToList();
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

