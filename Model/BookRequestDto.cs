using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Model
{
    public class BookRequestDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string BookNo { get; set; }
        public decimal BookPrice { get; set; }
        public int AvailableCopies { get; set; }
        public string BookDescription { get; set; }
        public string? BookFileUrl { get; set; }  // Chỉ cần URL, không còn IFormFile
        public List<string> ImageUrls { get; set; } // Danh sách URL ảnh
        public List<int> CategoryIds { get; set; } // Danh sách ID danh mục sách



    }




}
