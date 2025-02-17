using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class BookDto
    {
        [Required]
        public int BookId { get; set; } // Primary Key

        [Required(ErrorMessage = "Book Name is required")]
        [MaxLength(200, ErrorMessage = "Book Name cannot exceed 200 characters")]
        public string BookName { get; set; } = null!;

        [Required(ErrorMessage = "Category ID is required")]
        public int CatId { get; set; } // Foreign Key

        public string? CategoryName { get; set; } = null!; // Derived from CatId

        [Required(ErrorMessage = "Author ID is required")]
        public int AuthorId { get; set; } // Foreign Key

        public string? AuthorName { get; set; } = null!; // Derived from AuthorId

        [Range(1, int.MaxValue, ErrorMessage = "Book Number must be greater than zero")]
        public int BookNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Book Price must be greater than zero")]
        public int BookPrice { get; set; }
    }
}
