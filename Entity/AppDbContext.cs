using api_flms_service.Entity;
using api_flms_service.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api_flms_service.Entity
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }  // This represents the join table

        public DbSet<Loan> BookLoans { get; set; }
        public DbSet<Review> BookReviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<IssuedBook> IssuedBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship using the BookCategory join table
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });  // Composite key for the join table

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)  // 'BookCategories' collection in Book
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)  // 'BookCategories' collection in Category
                .HasForeignKey(bc => bc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure cascading deletes for other entities
            modelBuilder.Entity<Loan>()
                .HasOne(bl => bl.Book)
                .WithMany(b => b.BookLoans)
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loan>()
                .HasOne(bl => bl.User)
                .WithMany(u => u.BookLoans)
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(br => br.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(br => br.User)
                .WithMany()
                .HasForeignKey(br => br.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Apply DateTime conversion to UTC for all DateTime properties
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(), // Convert to UTC when saving to the database
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Read as UTC from the database
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v, // Convert to UTC when saving to the database
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v // Read as UTC from the database
            );

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Set table name to lowercase
                entity.SetTableName(entity.GetTableName()!.ToLower());

                // Apply DateTime conversion for all DateTime properties
                foreach (var property in entity.GetProperties())
                {
                    // Set column names to lowercase
                    property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName()!.ToLower(), null))!.ToLower());

                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
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
