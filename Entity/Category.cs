using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_flms_service.Entity
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        [JsonIgnore]
        public ICollection<Book> Books => BookCategories?.Select(bc => bc.Book).ToList() ?? new List<Book>();
    }
}