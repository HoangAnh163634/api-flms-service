using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Reviews
{
    public class IndexModel : PageModel
    {
        private readonly IReviewService _reviewService;

        public List<Review> Reviews { get; set; } = new List<Review>();

        public IndexModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Reviews = (await _reviewService.GetAllReviewsAsync()).ToList();
            return Page();
        }
    }
}