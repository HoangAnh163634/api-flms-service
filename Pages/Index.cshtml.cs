using api_auth_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AuthService _auth;
        public CurrentUser? CurrentUser { get; private set; }
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string _BaseUrl { get; set; }

        public IndexModel(AuthService authService, IConfiguration configuration)
        {
            _auth = authService;
            _BaseUrl = configuration["ApiBaseUrl"];
        }

        public async Task<IActionResult> OnGetAsync()
        {
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
