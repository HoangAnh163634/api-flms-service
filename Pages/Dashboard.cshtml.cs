using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace api_flms_service.Pages
{
    [AuthorizeAdmin]
    public class DashboardModel : PageModel
    {
        private readonly IAuthorService _authorService;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly ILoanService _loanService; // Thay IIssuedBookService bằng ILoanService
        private readonly IReviewService _reviewService;
        private readonly INotificationService _notificationService;

        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalLoans { get; set; } // Thay TotalIssuedBooks bằng TotalLoans
        public int TotalReviews { get; set; }
        public int TotalNotifications { get; set; }

        public DashboardModel(
            IAuthorService authorService,
            IUserService userService,
            IBookService bookService,
            ICategoryService categoryService,
            ILoanService loanService, // Thay IIssuedBookService bằng ILoanService
            IReviewService reviewService,
            INotificationService notificationService)
        {
            _authorService = authorService;
            _userService = userService;
            _bookService = bookService;
            _categoryService = categoryService;
            _loanService = loanService; // Khởi tạo ILoanService
            _reviewService = reviewService;
            _notificationService = notificationService;
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

            // Fetch loans count (thay thế issued books)
            var loans = await _loanService.GetAllLoansAsync(); // Lấy danh sách khoản mượn
            TotalLoans = loans.Count(); // Đếm tổng số khoản mượn

            // Fetch reviews count
            var reviews = await _reviewService.GetAllReviewsAsync();
            TotalReviews = reviews.Count();

            // Fetch notifications count
            var notifications = await _notificationService.GetAllNotificationsAsync();
            TotalNotifications = notifications.Count();
        }
    }
}