using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class BookRequestDto
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Book name is required.")]
        public string BookName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Author ID must be greater than 0.")]
        public int AuthorId { get; set; }

        public string? AuthorName { get; set; }

        [Required(ErrorMessage = "At least one category is required.")]
        public List<int>? CategoryIds { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        public string BookNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publication year is required.")]
        [Range(1, 9999, ErrorMessage = "Publication year must be between 1 and 9999.")]
        public int BookPrice { get; set; }

        [Required(ErrorMessage = "Available copies is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available copies must be 0 or greater.")]
        public int AvailableCopies { get; set; }

        public string BookDescription { get; set; } = string.Empty;

        public string? BookFileUrl { get; set; }

        public List<string>? ImageUrls { get; set; }
    }
}