using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Books
{
    [AuthorizeUser]
    public class ManageModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public ManageModel(IBookService bookService, IAuthorService authorService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public BookDto Book { get; set; } = new BookDto();
        public int? BookId { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            BookId = id;

            Authors = (await _authorService.GetAllAuthorsAsync()).ToList() ?? new List<Author>();
            Categories = (await _categoryService.GetAllCategoriesAsync()).ToList() ?? new List<Category>();

            if (id.HasValue)
            {
                var book = await _bookService.GetBookByIdAsync(id.Value);
                if (book == null)
                {
                    return NotFound();
                }
                Book = new BookDto
                {
                    BookId = book.BookId,
                    BookName = book.Title ?? string.Empty,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author?.Name ?? string.Empty,
                    Categories = book.BookCategories?.Select(bc => new CategoryDto
                    {
                        CategoryId = bc.Category.CategoryId,
                        CategoryName = bc.Category.CategoryName
                    }).ToList() ?? new List<CategoryDto>(),
                    BookNo = book.ISBN ?? string.Empty,
                    BookPrice = book.PublicationYear,
                    BookDescription = book.BookDescription ?? string.Empty,
                    AvailableCopies = book.AvailableCopies,
                    ImageUrls = book.ImageUrls ?? string.Empty,
                    BookFileUrl = book.BookFileUrl
                };
            }

            return Page();
        }
    }
}