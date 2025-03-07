using api_auth_service.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

[IgnoreAntiforgeryToken] // Disable CSRF protection for API-like request
public class LogoutModel : PageModel
{
    private readonly AuthService _authService;

    public string LogoutUrl { get; set; }

    public LogoutModel(AuthService authService)
    {
        _authService = authService;
        
    }

    public async Task<IActionResult> OnGetAsync()
    {
        LogoutUrl = await _authService.GetLogoutUrl();
        return Page(); // Show logout page
    }

    public IActionResult OnPost()
    {
        _authService.HandleLogout(HttpContext.Request, HttpContext.Response);

        // Redirect after handling logout
        return RedirectToPage(_authService.GetLogoutUrl());
    }
}
