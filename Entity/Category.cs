using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_flms_service.Entity
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Khởi tạo mặc định

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>(); // Đã khởi tạo
        [JsonIgnore] // Thêm để tránh vòng lặp JSON serialization
        public ICollection<Book> Books => BookCategories.Select(bc => bc.Book).ToList(); // Navigation property
    }
}