using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string? Biography { get; set; }
        public string? CloudinaryId { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? Name { get; set; }

        public ICollection<Book>? Books { get; set; }
    }
}
