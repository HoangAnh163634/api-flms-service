using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace api_flms_service.Pages.Authors
{
    public class ManageModel : PageModel
    {
        private readonly IAuthorService _authorService;

        public ManageModel(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [BindProperty]
        public Author Author { get; set; } = new Author(); // Định nghĩa thuộc tính Author

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Author = await _authorService.GetAuthorByIdAsync(id.Value);
                if (Author == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Author = new Author();
            }

            return Page();
        }
    }
}