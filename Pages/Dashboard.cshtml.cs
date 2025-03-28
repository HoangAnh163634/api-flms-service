using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
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
        private readonly ILoanService _loanService;
        private readonly IReviewService _reviewService;
        private readonly INotificationService _notificationService;

        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalLoans { get; set; }
        public int TotalReviews { get; set; }
        public int TotalNotifications { get; set; }
        public int TotalReserves { get; set; } // Thêm thuộc tính cho số lượng sách đặt trước

        public DashboardModel(
            IAuthorService authorService,
            IUserService userService,
            IBookService bookService,
            ICategoryService categoryService,
            ILoanService loanService,
            IReviewService reviewService,
            INotificationService notificationService)
        {
            _authorService = authorService;
            _userService = userService;
            _bookService = bookService;
            _categoryService = categoryService;
            _loanService = loanService;
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

            // Fetch loans count
            var loans = await _loanService.GetAllLoansAsync();
            TotalLoans = loans.Count(l => l.LoanDate < DateTime.MaxValue.AddDays(-1)); // Chỉ đếm các bản ghi "loan" thực sự

            // Fetch reserves count
            TotalReserves = loans.Count(l => l.LoanDate >= DateTime.MaxValue.AddDays(-1) && l.ReturnDate == null); // Đếm các bản ghi "reserve"

            // Fetch reviews count
            var reviews = await _reviewService.GetAllReviewsAsync();
            TotalReviews = reviews.Count();

            // Fetch notifications count
            var notifications = await _notificationService.GetAllNotificationsAsync();
            TotalNotifications = notifications.Count();
        }
    }
}