using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using api_flms_service.Model;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;

namespace api_flms_service.Pages.Books
{
    [BindProperties(SupportsGet = true)]
    public class ShowBooksModel : PageModel
    {

        private IBookService _book;
        private IBookCategoryService _category;

        public ShowBooksModel(IBookService book, IBookCategoryService category)
        {
            _book = book;
            _category = category;
        }

        public String Title { get; set; } 
        public String Category { get; set; } 

        public List<Book> Book { get;set; } = default!;
        public List<Book> Books { get; set; } = default!;
        public List<Book> Books2 { get; set; } = default!;
        public List<BookCategory> BookCategories { get; set; } = default!;


        public async Task OnGetAsync()
        {
            Books = (await _book.GetAllBooksAsync()).ToList();
            BookCategories = (await _category.GetAllBooksCategoriesAsync()).ToList();

            if (!string.IsNullOrEmpty(Category))
            {
                // Filter books by category
                Books2 = Books.Where(book => book.Categories.Any(c => c.CategoryName == Category)).ToList();
                await Console.Out.WriteLineAsync("HERE1");
            }
            else
            {
                Books2 = Books;
                await Console.Out.WriteLineAsync("HERE2");

            }

            if (!string.IsNullOrEmpty(Title))
            {
                // Filter books by title
                Book.AddRange(Books2.Where(book => book.Title.Contains(Title)));
                await Console.Out.WriteLineAsync("HERE3");

            }
            else
            {
                Book = Books2;
                await Console.Out.WriteLineAsync("HERE4");

            }
            await Console.Out.WriteLineAsync(Title);
            await Console.Out.WriteLineAsync(Category);



        }
    }
}
