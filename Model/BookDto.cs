using api_flms_service.Entity;

namespace api_flms_service.Model
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<Category> Category { get; set; } = new List<Category>(); // Sử dụng api_flms_service.Entity.Category
        public string BookNo { get; set; }
        public int BookPrice { get; set; }
        public int AvailableCopies { get; set; }
        public string BookDescription { get; set; }
        public string CloudinaryImageId { get; set; }
        public string ImageUrls { get; set; }
    }
}