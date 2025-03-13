using api_flms_service.Entity;
using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Mobile is required.")]
        [Phone(ErrorMessage = "Invalid mobile number.")]
        public string? PhoneNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public string? Role { get; set; }

        public ICollection<Loan>? BookLoans { get; set; } = new List<Loan>();
        public ICollection<Review>? BookReviews { get; set; } = new List<Review>();
    }
}