using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Model
{
    //public class BookRequestDto
    //{
    //    //public int BookId { get; set; }
    //    //public string BookName { get; set; }
    //    //public int AuthorId { get; set; }
    //    //public string AuthorName { get; set; } = string.Empty;
    //    //public string BookNo { get; set; }
    //    //public decimal BookPrice { get; set; }
    //    //public int AvailableCopies { get; set; }
    //    //public string BookDescription { get; set; }
    //    //public string? BookFileUrl { get; set; }  // Chỉ cần URL, không còn IFormFile
    //    //public List<string> ImageUrls { get; set; } // Danh sách URL ảnh
    //    //public List<int> CategoryIds { get; set; } // Danh sách ID danh mục sách



    //}


    public class BookRequestDto
    {
        [FromForm]
        public int BookId { get; set; }

        [FromForm]
        public string BookName { get; set; }

        [FromForm]
        public int AuthorId { get; set; }

        [FromForm]
        public string AuthorName { get; set; } = string.Empty;

        [FromForm]
        public string BookNo { get; set; }

        [FromForm]
        public decimal BookPrice { get; set; }

        [FromForm]
        public int AvailableCopies { get; set; }

        [FromForm]
        public string BookDescription { get; set; }

        [FromForm]
        public string? BookFileUrl { get; set; }

        [FromForm]
        public List<string>? ImageUrls { get; set; } // Xem xét chuyển thành string

        [FromForm]
        public List<int>? CategoryIds { get; set; } // Xem xét chuyển thành string
    }

}
