using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_flms_service.Entity
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int AvailableCopies { get; set; }
        public string? BookDescription { get; set; }
        public string CloudinaryImageId { get; set; }
        public string? ISBN { get; set; }
        public int PublicationYear { get; set; }
        public string? Title { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();  // Many-to-many relationship with Category
        public ICollection<Category> Categories => BookCategories.Select(bc => bc.Category).ToList();  // Navigation property for easy access

        public ICollection<Loan>? BookLoans { get; set; } = new List<Loan>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }

}
