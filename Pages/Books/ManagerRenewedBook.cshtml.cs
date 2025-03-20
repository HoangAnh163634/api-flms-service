using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Books
{
    [AuthorizeUser]
    public class ManagerRenewedBookModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
