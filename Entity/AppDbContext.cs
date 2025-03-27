using api_flms_service.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<IssuedBook> IssuedBooks { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ N-N sử dụng BookCategory
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Categories)
            .WithMany(c => c.Books)
            .UsingEntity<BookCategory>(
                j => j.HasOne(bc => bc.Category)
                      .WithMany()
                      .HasForeignKey(bc => bc.CategoryId),
                j => j.HasOne(bc => bc.Book)
                      .WithMany()
                      .HasForeignKey(bc => bc.BookId),
                j =>
                {
                    j.HasKey(bc => new { bc.BookId, bc.CategoryId });
                    j.ToTable("bookcategory");
                });

        // Cấu hình các quan hệ khác
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
            .WithMany(u => u.BookReviews)
            .HasForeignKey(br => br.UserId)
            .HasConstraintName("fk_reviews_users_userid")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .Property(r => r.UserId)
            .HasColumnName("userid");

        modelBuilder.Entity<Review>()
            .Ignore("UserId1");

        modelBuilder.Entity<BookCategory>()
            .Property(bc => bc.BookId)
            .HasColumnName("bookid");

        modelBuilder.Entity<BookCategory>()
            .Property(bc => bc.CategoryId)
            .HasColumnName("categoryid");

        modelBuilder.Entity<BookCategory>()
            .Ignore("BookId1");

        modelBuilder.Entity<BookCategory>()
            .Ignore("CategoryId1");

        // Cấu hình ánh xạ cho bảng notifications
        modelBuilder.Entity<Notification>()
            .ToTable("notifications")
            .HasKey(n => n.Id);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Notification>()
            .Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Content)
            .HasColumnName("content")
            .HasMaxLength(1000);

        modelBuilder.Entity<Notification>()
            .Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone"); // Sửa thành timestamp with time zone

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Notification>()
            .Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false);

        // Thiết lập quan hệ 1-nhiều giữa Author và Book
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa tác giả sẽ xóa luôn sách

        // Cấu hình lowercase tên bảng và cột
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName()!.ToLower(), null))!.ToLower());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()!.ToLower());
            }
        }

        //Fix timestamp
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }
    }
}