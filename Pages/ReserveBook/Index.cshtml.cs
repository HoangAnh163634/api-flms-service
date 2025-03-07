using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.ReserveBook
{
    public class IndexModel : PageModel
    {
        private readonly IReserveBookService _reserveBookService;

        public List<Book> Books { get; set; } = new List<Book>();

        public IndexModel(IReserveBookService reserveBookService)
        {
            _reserveBookService = reserveBookService;
        }

        public async Task OnGet()
        {
            Books = (await _reserveBookService.GetAllBooksAsync()).ToList();
        }
    }
}

