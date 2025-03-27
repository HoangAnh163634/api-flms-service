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
        private readonly IIssuedBookService _issuedBookService;
        private readonly IReviewService _reviewService;
        private readonly INotificationService _notificationService; // Thêm INotificationService

        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalIssuedBooks { get; set; }
        public int TotalReviews { get; set; }
        public int TotalNotifications { get; set; } // Thêm thuộc tính TotalNotifications

        public DashboardModel(
            IAuthorService authorService,
            IUserService userService,
            IBookService bookService,
            ICategoryService categoryService,
            IIssuedBookService issuedBookService,
            IReviewService reviewService,
            INotificationService notificationService) // Thêm INotificationService vào constructor
        {
            _authorService = authorService;
            _userService = userService;
            _bookService = bookService;
            _categoryService = categoryService;
            _issuedBookService = issuedBookService;
            _reviewService = reviewService;
            _notificationService = notificationService; // Khởi tạo INotificationService
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

            // Fetch reviews count
            var reviews = await _reviewService.GetAllReviewsAsync();
            TotalReviews = reviews.Count();

            // Fetch notifications count
            var notifications = await _notificationService.GetAllNotificationsAsync(); // Lấy danh sách thông báo
            TotalNotifications = notifications.Count(); // Đếm tổng số thông báo
        }
    }
}