using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api_flms_service.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookcategory> Bookcategories { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Issuedbook> Issuedbooks { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.thkntxafxhfymhzptblp;Password=database_flms_dev;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=flms_dev");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Authorid).HasName("pk_authors");

            entity.ToTable("authors");

            entity.Property(e => e.Authorid).HasColumnName("authorid");
            entity.Property(e => e.Biography).HasColumnName("biography");
            entity.Property(e => e.Cloudinaryid).HasColumnName("cloudinaryid");
            entity.Property(e => e.Countryoforigin).HasColumnName("countryoforigin");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Bookid).HasName("pk_books");

            entity.ToTable("books");

            entity.HasIndex(e => e.Authorid, "IX_books_authorid");

            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Authorid).HasColumnName("authorid");
            entity.Property(e => e.Availablecopies).HasColumnName("availablecopies");
            entity.Property(e => e.Bookdescription)
                .HasDefaultValueSql("''::text")
                .HasColumnName("bookdescription");
            entity.Property(e => e.Bookfileurl)
                .HasDefaultValueSql("''::text")
                .HasColumnName("bookfileurl");
            entity.Property(e => e.Borroweduntil).HasColumnName("borroweduntil");
            entity.Property(e => e.Cloudinaryimageid).HasColumnName("cloudinaryimageid");
            entity.Property(e => e.Imageurls).HasColumnName("imageurls");
            entity.Property(e => e.Isbn)
                .HasDefaultValueSql("''::text")
                .HasColumnName("isbn");
            entity.Property(e => e.Publicationyear).HasColumnName("publicationyear");
            entity.Property(e => e.Title)
                .HasDefaultValueSql("''::text")
                .HasColumnName("title");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Author).WithMany(p => p.Books).HasForeignKey(d => d.Authorid);
        });

        modelBuilder.Entity<Bookcategory>(entity =>
        {
            entity.HasKey(e => new { e.Bookid, e.Categoryid }).HasName("pk_bookcategory");

            entity.ToTable("bookcategory");

            entity.HasIndex(e => e.Bookid2, "IX_bookcategory_bookid2");

            entity.HasIndex(e => e.Categoryid, "IX_bookcategory_categoryid");

            entity.HasIndex(e => e.Categoryid2, "IX_bookcategory_categoryid2");

            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Bookid2).HasColumnName("bookid2");
            entity.Property(e => e.Categoryid2).HasColumnName("categoryid2");

            entity.HasOne(d => d.Book).WithMany(p => p.BookcategoryBooks).HasForeignKey(d => d.Bookid);

            entity.HasOne(d => d.Bookid2Navigation).WithMany(p => p.BookcategoryBookid2Navigations).HasForeignKey(d => d.Bookid2);

            entity.HasOne(d => d.Category).WithMany(p => p.BookcategoryCategories).HasForeignKey(d => d.Categoryid);

            entity.HasOne(d => d.Categoryid2Navigation).WithMany(p => p.BookcategoryCategoryid2Navigations).HasForeignKey(d => d.Categoryid2);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("pk_categories");

            entity.ToTable("categories");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasDefaultValueSql("''::text")
                .HasColumnName("categoryname");
        });

        modelBuilder.Entity<Issuedbook>(entity =>
        {
            entity.HasKey(e => e.Sno).HasName("pk_issuedbooks");

            entity.ToTable("issuedbooks");

            entity.Property(e => e.Sno).HasColumnName("sno");
            entity.Property(e => e.Bookauthor).HasColumnName("bookauthor");
            entity.Property(e => e.Bookname).HasColumnName("bookname");
            entity.Property(e => e.Bookno).HasColumnName("bookno");
            entity.Property(e => e.Issuedate).HasColumnName("issuedate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Studentid).HasColumnName("studentid");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Bookloanid).HasName("pk_loans");

            entity.ToTable("loans");

            entity.HasIndex(e => e.Bookid, "IX_loans_bookid");

            entity.HasIndex(e => e.Userid, "IX_loans_userid");

            entity.Property(e => e.Bookloanid).HasColumnName("bookloanid");
            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Loandate).HasColumnName("loandate");
            entity.Property(e => e.Returndate).HasColumnName("returndate");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Book).WithMany(p => p.Loans).HasForeignKey(d => d.Bookid);

            entity.HasOne(d => d.User).WithMany(p => p.Loans).HasForeignKey(d => d.Userid);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("pk_reviews");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.Bookid, "IX_reviews_bookid");

            entity.HasIndex(e => e.Userid, "IX_reviews_userid");

            entity.Property(e => e.Reviewid).HasColumnName("reviewid");
            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Reviewdate).HasColumnName("reviewdate");
            entity.Property(e => e.Reviewtext).HasColumnName("reviewtext");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews).HasForeignKey(d => d.Bookid);

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_reviews_users_userid");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("pk_users");

            entity.ToTable("users");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phonenumber).HasColumnName("phonenumber");
            entity.Property(e => e.Registrationdate).HasColumnName("registrationdate");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
