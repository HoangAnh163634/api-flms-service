using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace api_flms_service.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IAuthorService _authorService;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IIssuedBookService _issuedBookService;

        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalIssuedBooks { get; set; }

        public DashboardModel(
            IAuthorService authorService,
            IUserService userService,
            IBookService bookService,
            ICategoryService categoryService,
            IIssuedBookService issuedBookService)
        {
            _authorService = authorService;
            _userService = userService;
            _bookService = bookService;
            _categoryService = categoryService;
            _issuedBookService = issuedBookService;
        }

        public async Task OnGetAsync()
        {
            // Fetch authors count
            var authors = await _authorService.GetAllAuthorsAsync();
            TotalAuthors = authors.Count();

            // Fetch users count
            var users = await _userService.GetAllUsersAsync();
            TotalUsers = users.Count();

            // Fetch books count
            var books = await _bookService.GetAllBooksAsync();
            TotalBooks = books.Count();

            // Fetch categories count
            var categories = await _categoryService.GetAllCategoriesAsync();
            TotalCategories = categories.Count();

            // Fetch issued books count
            var issuedBooks = await _issuedBookService.GetAllIssuedBooksAsync();
            TotalIssuedBooks = issuedBooks.Count();
        }
    }
   
}
