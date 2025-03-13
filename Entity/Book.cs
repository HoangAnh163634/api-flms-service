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

        public DateTime BorrowedUntil { get; set; } // Thêm trường này
        public int UserId { get; set; } // Thêm UserId để liên kết sách với người dùng
        // Lưu đường dẫn ảnh dưới dạng chuỗi, các đường dẫn được phân cách bởi dấu cách hoặc dấu phẩy
        public string ImageUrls { get; set; } = ""; // Lưu chuỗi đường dẫn ảnh, ví dụ "image1.jpg image2.jpg image3.jpg"

        public ICollection<Loan>? BookLoans { get; set; } = new List<Loan>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();

    }

}
