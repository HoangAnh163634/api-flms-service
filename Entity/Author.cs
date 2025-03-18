using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string? Biography { get; set; } = string.Empty; // Khởi tạo mặc định
        public string? CloudinaryId { get; set; } = string.Empty; // Khởi tạo mặc định
        public string? CountryOfOrigin { get; set; } = string.Empty; // Khởi tạo mặc định
        public string? Name { get; set; } = string.Empty; // Khởi tạo mặc định

        public ICollection<Book> Books { get; set; } = new List<Book>(); // Khởi tạo mặc định
    }
}