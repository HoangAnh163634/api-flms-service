using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api_flms_service.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public List<Category> Categories { get; set; } = new List<Category>();

        public CategoryModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task OnGet()
        {
            Categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
        }
    }

}
