using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using api_auth_service.Services;

public class AuthorizeAdminAttribute : TypeFilterAttribute
{
    public AuthorizeAdminAttribute() : base(typeof(AuthorizeAdminFilter))
    {
        Arguments = new object[] { };
    }
}

public class AuthorizeAdminFilter : IAsyncAuthorizationFilter
{
    private readonly IUserService _userService;
    private readonly AuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizeAdminFilter(IUserService userService, AuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null || string.IsNullOrEmpty(user.Email) || !await _userService.IsAuthenticatedAdmin(user.Email))
        {
            context.Result = new ViewResult { ViewName = "AccessDenied" };
        }
    }
}
