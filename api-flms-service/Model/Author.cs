using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; } // Primary Key
        public string AuthorName { get; set; } = null!;
    }
}
