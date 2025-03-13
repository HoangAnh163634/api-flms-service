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
    public class SeeAuthorsModel : PageModel
    {
        public IAuthorService _author;

        public SeeAuthorsModel(IAuthorService author)
        {
            _author = author;
        }

        public IEnumerable<Author> Author { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Author = await _author.GetAllAuthorsAsync();
        }
    }
}
