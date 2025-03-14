using api_flms_service.Model;
using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        public DateTime? ReviewDate { get; set; }

        [Required(ErrorMessage = "Review text is required.")]
        public string? ReviewText { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}