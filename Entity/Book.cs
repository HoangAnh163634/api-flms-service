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
        public string BookDescription { get; set; } = string.Empty;
        public string CloudinaryImageId { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        [JsonIgnore]
        public ICollection<Category> Categories => BookCategories?.Select(bc => bc.Category).ToList() ?? new List<Category>();

        public DateTime BorrowedUntil { get; set; } = DateTime.MinValue;
        public int UserId { get; set; } = 0;
        public string ImageUrls { get; set; } = string.Empty;

        public ICollection<Loan> BookLoans { get; set; } = new List<Loan>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public string BookFileUrl { get; set; } = string.Empty;
    }
}