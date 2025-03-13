using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages.Reviews
{
    public class EditModel : PageModel
    {
        private readonly IReviewService _reviewService;

        [BindProperty]
        public Review Review { get; set; } = new Review();

        public EditModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Review = await _reviewService.GetReviewByIdAsync(id);
            if (Review == null)
            {
                return NotFound();
            }
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

            var updatedReview = await _reviewService.UpdateReviewAsync(Review);
            if (updatedReview == null)
            {
                ModelState.AddModelError("", "Failed to update review.");
                return Page();
            }

            return RedirectToPage("/Reviews/Index");
        }
    }
}