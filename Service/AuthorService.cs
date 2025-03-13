using api_flms_service.Entity;
using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _dbContext;

        public AuthorService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _dbContext.Authors.ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _dbContext.Authors
                                   .Include(e => e.Books)
                                   .FirstOrDefaultAsync(e => e.AuthorId == id);  // Use FirstOrDefaultAsync for non-primary key queries
        }


        public async Task<Author> AddAuthorAsync(Author author)
        {
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();
            return author;
        }

        public async Task<Author?> UpdateAuthorAsync(Author author)
        {
            var existingAuthor = await _dbContext.Authors.FindAsync(author.AuthorId);
            if (existingAuthor == null) return null;

            existingAuthor.Name = author.Name;
            await _dbContext.SaveChangesAsync();
            return existingAuthor;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null) return false;

            _dbContext.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
