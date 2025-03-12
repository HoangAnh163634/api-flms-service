using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using api_flms_service.Model;
using api_flms_service.Entity;

namespace api_flms_service.Pages.BookLoans
{
    public class ListModel : PageModel
    {
        
        //private readonly api_flms_service.Model.DatabaseContext _context;

        /*public ListModel(api_flms_service.Model.DatabaseContext context)
        {
            _context = context;
        }*/

        public List<Loan> BookLoan { get;set; } = default!;
        [BindProperty]
        public List<Loan> BookLoanPast { get; set; } = default!;
        public List<Loan> BookLoanCurrent { get; set; } = default!;
        public Author author { get; set; }
        [BindProperty]
        public string opinion {  get; set; }
        [BindProperty]
        public string rating { get; set; }
        [BindProperty]
        public List<Review> BookReviews { get; set; } = default!;
        [BindProperty]
        public User user { get; set; } = default!;

        public async Task OnGetAsync()
        {
            int id = (int)HttpContext.Session.GetInt32("id");

            /*if (_context.Users != null)
            {
                user = await _context.Users
                    .Include(b => b.BookLoans)
                        .ThenInclude(b1 => b1.Book)
                            .ThenInclude(b2 => b2.Author).SingleOrDefaultAsync(b => b.UserId == id);
            }
            BookReviews = await _context.BookReviews.Include(b => b.User).Include(b => b.Book).ToListAsync();
            BookLoan = (List<BookLoan>)user.BookLoans;
            BookLoanCurrent = new List<BookLoan>();
            BookLoanPast = new List<BookLoan>();
            foreach (var item in BookLoan)
            {
                if(item.LoanDate.Equals(item.ReturnDate))
                {
                    BookLoanCurrent.Add(item);
                }
                else { BookLoanPast.Add(item); }
            }
*/
            
        }

        public async Task<IActionResult> OnPostAsync(int id, string handler)
        {
            /*BookReviews = await _context.BookReviews.Include(b => b.User).Include(b => b.Book).ToListAsync();

            var bookloan = await _context.BookLoans.Include(b => b.Book).FirstOrDefaultAsync(m => m.BookLoanId == id);

            if (handler == "handler")
            {
                bookloan.ReturnDate = DateTime.UtcNow;
                var book = bookloan.Book;
                book.AvailableCopies = 1;
                await _context.SaveChangesAsync();
            }
            
            if (_context.Users != null)
            {
                user = await _context.Users.Include(b => b.BookLoans).ThenInclude(b1 => b1.Book).ThenInclude(b2 => b2.Author).SingleOrDefaultAsync(b => b.UserId == (int)HttpContext.Session.GetInt32("id"));
            }
            BookLoan = (List<BookLoan>)user.BookLoans;
            BookLoanCurrent = new List<BookLoan>();
            BookLoanPast = new List<BookLoan>();
            foreach (var item in BookLoan)
            {
                if (item.LoanDate.Equals(item.ReturnDate))
                {
                    BookLoanCurrent.Add(item);
                }
                else { BookLoanPast.Add(item); }
            }*/
            
            
            return Page();
        }
    }
}
