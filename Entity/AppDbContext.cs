using api_flms_service.Entity;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ N-N sử dụng BookCategory nhưng không tạo bảng riêng
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
                    j.HasKey(bc => new { bc.BookId, bc.CategoryId }); // Composite key
                    j.ToTable("bookcategory"); // Đặt tên bảng mong muốn
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
            .WithMany()
            .HasForeignKey(br => br.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Giữ nguyên các cấu hình DateTime và lowercase tên bảng
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLower());

            foreach (var property in entity.GetProperties())
            {
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

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()!.ToLower());
            }
        }
    }
}
