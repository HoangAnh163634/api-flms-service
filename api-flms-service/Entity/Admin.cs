using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class Admin
    {
        [Key]
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public long Mobile { get; set; }
    }

}
