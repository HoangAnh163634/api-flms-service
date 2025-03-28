using api_flms_service.Entity;

namespace api_flms_service.Model
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public string BookNo { get; set; } = string.Empty;
        public int BookPrice { get; set; }
        public int AvailableCopies { get; set; }
        public string BookDescription { get; set; } = string.Empty;
        public string CloudinaryImageId { get; set; } = string.Empty;
        public string ImageUrls { get; set; } = string.Empty;
        public string? BookFileUrl { get; set; } 

    }
}