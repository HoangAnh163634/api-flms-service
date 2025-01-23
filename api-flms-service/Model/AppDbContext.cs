using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api_flms_service.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define DbSet for each table
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IssuedBook> IssuedBooks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure all tables and columns to use lowercase names
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Set table name to lowercase
                entity.SetTableName(entity.GetTableName()!.ToLower());

                // Set column names to lowercase
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName()!.ToLower(), null))!.ToLower());
                }

                // Set key names to lowercase (optional)
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()!.ToLower());
                }
            }
        }

    }

}
