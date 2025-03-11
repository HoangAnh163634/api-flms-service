using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using api_flms_service.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using api_flms_service.ServiceInterface.api_flms_service.Services;
using api_flms_service.ServiceInterface;
using api_auth_service.Services;

public class AuthorizeUserAttribute : TypeFilterAttribute
{
    public AuthorizeUserAttribute() : base(typeof(AuthorizeUserFilter))
    {
    }
}

public class AuthorizeUserFilter : IAsyncAuthorizationFilter
{
    private readonly IUserService _userService;
    private readonly AuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizeUserFilter(IUserService userService, AuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _authService = authService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null || string.IsNullOrEmpty(user.Email) || !await _userService.IsAuthenticatedAdmin(user.Email))
        {
            // Show the permission denied page without redirect
            context.Result = new ViewResult { ViewName = "AccessDenied" };
        }
    }
}
