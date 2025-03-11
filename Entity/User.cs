using api_flms_service.Entity;
using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Role { get; set; }

        public ICollection<Loan> BookLoans { get; set; }
        public ICollection<Review> BookReviews { get; set; }
    }
}