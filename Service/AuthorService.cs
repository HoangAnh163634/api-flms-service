using api_flms_service.Entity;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_flms_service.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;

        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors
                .Include(a => a.Books) // Bao gồm danh sách sách
                .ToListAsync();
        }

        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors
                .Include(a => a.Books) // Bao gồm danh sách sách
                .FirstOrDefaultAsync(a => a.AuthorId == id);
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            var existingAuthor = await _context.Authors
                .Include(a => a.Books) // Bao gồm danh sách sách để cập nhật
                .FirstOrDefaultAsync(a => a.AuthorId == author.AuthorId);

            if (existingAuthor == null) return null;

            // Cập nhật thông tin cơ bản
            existingAuthor.Name = author.Name;
            existingAuthor.Biography = author.Biography;
            existingAuthor.CountryOfOrigin = author.CountryOfOrigin;
            existingAuthor.CloudinaryId = author.CloudinaryId;

            // Cập nhật danh sách sách
            existingAuthor.Books.Clear(); // Xóa sách cũ
            if (author.Books != null)
            {
                foreach (var book in author.Books)
                {
                    book.AuthorId = existingAuthor.AuthorId;
                    existingAuthor.Books.Add(book);
                }
            }

            await _context.SaveChangesAsync();
            return existingAuthor;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}