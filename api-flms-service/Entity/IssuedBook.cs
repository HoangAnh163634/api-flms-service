using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Model
{
    public class IssuedBook
    {
        [Key]
        public int SNo { get; set; } // Primary Key
        public int BookNo { get; set; }
        public string BookName { get; set; } = null!;
        public string BookAuthor { get; set; } = null!;
        public int StudentId { get; set; }
        public int Status { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
    }

}
