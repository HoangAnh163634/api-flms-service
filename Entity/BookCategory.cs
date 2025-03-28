using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}