using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class BookCategory
    {
        //[Key]
        public int BookId { get; set; }
        public Book Book { get; set; } = new Book(); // Khởi tạo mặc định

        public int CategoryId { get; set; }
        public Category Category { get; set; } = new Category(); // Khởi tạo mặc định
    }
}