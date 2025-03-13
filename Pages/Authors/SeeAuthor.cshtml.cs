using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Pages.Authors
{
    public class SeeAuthorModel : PageModel
    {
        public IAuthorService _author;

        public SeeAuthorModel(IAuthorService author)
        {
            _author = author;
        }

        public Author Author { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _author.GetAuthorByIdAsync((int)id);
            if (author == null)
            {
                return NotFound();
            }
            else 
            {
                Author = author;
            }
            return Page();
        }
    }
}
