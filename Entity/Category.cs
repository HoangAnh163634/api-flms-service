using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();  // Many-to-many relationship with Book
        public ICollection<Book> Books => BookCategories.Select(bc => bc.Book).ToList();  // Navigation property for easy access
    }
}
