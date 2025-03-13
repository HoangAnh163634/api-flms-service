using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class IssuedBook
    {
        [Key]
        public int SNo { get; set; }
        public string? BookNo { get; set; }
        public string? BookName { get; set; }
        public string? BookAuthor { get; set; }
        public int StudentId { get; set; }
        public string? Status { get; set; }
        public DateTime? IssueDate { get; set; }
    }
}
