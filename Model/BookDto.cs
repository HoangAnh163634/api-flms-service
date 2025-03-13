using api_flms_service.Entity;
using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<Category> Category { get; set; }  // List of categories associated with the book
        public string BookNo { get; set; }
        public int BookPrice { get; set; }
    }

}
