using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Reviews
{
    public class AddModel : PageModel
    {
        private readonly IReviewService _reviewService;

        [BindProperty]
        public Review Review { get; set; } = new Review();

        public AddModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            try
            {
                var addedReview = await _reviewService.AddReviewAsync(Review);
                if (addedReview == null)
                {
                    ModelState.AddModelError("", "Failed to add review.");
                    return Page();
                }
                return RedirectToPage("/Reviews/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}