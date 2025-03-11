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
        public int? Rating { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ReviewText { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
