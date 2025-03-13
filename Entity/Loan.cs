using api_flms_service.Model;
using System.ComponentModel.DataAnnotations;

namespace api_flms_service.Entity
{
    public class Loan
    {
        [Key]
        public int BookLoanId { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public DateTime? LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
