using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_flms_service.Model
{
    public class Book
    {
        [Key]
        public int BookId { get; set; } // Primary Key
        public string BookName { get; set; } = null!;
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; } // Foreign Key
        [ForeignKey(nameof(Category))]
        public int CatId { get; set; } // Foreign Key
        public int BookNo { get; set; }
        public int BookPrice { get; set; }

        // Navigation Properties
        public Author Author { get; set; } = null!;
        public Category Category { get; set; } = null!;

        public DateTime BorrowedUntil { get; set; } // Thêm trường này
        public int UserId { get; set; } // Thêm UserId để liên kết sách với người dùng

        // Lưu đường dẫn ảnh dưới dạng chuỗi, các đường dẫn được phân cách bởi dấu cách hoặc dấu phẩy
        public string ImageUrls { get; set; } = ""; // Lưu chuỗi đường dẫn ảnh, ví dụ "image1.jpg image2.jpg image3.jpg"
    }
}
