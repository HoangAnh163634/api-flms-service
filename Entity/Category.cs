using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class Category
    {
        [Key]
        public int CatId { get; set; } // Primary Key
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        public string CatName { get; set; } = null!;
    }

}
