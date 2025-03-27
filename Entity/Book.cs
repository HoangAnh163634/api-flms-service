using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api_flms_service.Entity
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int AvailableCopies { get; set; }
        public string BookDescription { get; set; } = string.Empty; // Khởi tạo mặc định
        public string CloudinaryImageId { get; set; } = string.Empty; // Khởi tạo mặc định
        public string ISBN { get; set; } = string.Empty; // Khởi tạo mặc định
        public int PublicationYear { get; set; }
        public string Title { get; set; } = string.Empty; // Khởi tạo mặc định

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        [JsonIgnore]
        public ICollection<Category> Categories => BookCategories.Select(bc => bc.Category).ToList();

        public DateTime BorrowedUntil { get; set; } = DateTime.MinValue; // Khởi tạo mặc định
        public int UserId { get; set; } = 0; // Khởi tạo mặc định
        public string ImageUrls { get; set; } = string.Empty; // Khởi tạo mặc định

        public ICollection<Loan> BookLoans { get; set; } = new List<Loan>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public string BookFileUrl { get; set; } = string.Empty;
    }
}