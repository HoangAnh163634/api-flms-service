using api_auth_service.Service;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using api_flms_service.ServiceInterface.api_flms_service.Services;
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

        public IndexModel(AuthService authService)
        {
            _auth = authService;
        }

        public async Task OnGetAsync()
        {
            CurrentUser = await _auth.GetCurrentUserAsync();
            LoginUrl = await _auth.GetLoginUrl(Request.GetEncodedUrl());
            LogoutUrl = await _auth.GetLogoutUrl(Request.GetEncodedUrl());
        }

    }
}
