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

        public IndexModel(IAuthorService author, IBookService book)
        {
            _author = author;
            _book = book;
        }

        [BindProperty]
        public List<Entity.Author> authors { get; set; }
        [BindProperty]
        public List<Book> books { get; set; }

        public async Task OnGet()
        {
            authors = await _author.GetAllAuthorsAsync();
            books = (await _book.GetAllBooksAsync()).ToList();
        }

    }
}
