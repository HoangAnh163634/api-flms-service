using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public long Mobile { get; set; }
        public string Address { get; set; } = null!;

        public string? GoogleId { get; set; } 
    }

}
