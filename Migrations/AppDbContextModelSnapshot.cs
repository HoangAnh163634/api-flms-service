﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_flms_service.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("api_flms_service.Entity.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("authorid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AuthorId"));

                    b.Property<string>("Biography")
                        .HasColumnType("text")
                        .HasColumnName("biography");

                    b.Property<string>("CloudinaryId")
                        .HasColumnType("text")
                        .HasColumnName("cloudinaryid");

                    b.Property<string>("CountryOfOrigin")
                        .HasColumnType("text")
                        .HasColumnName("countryoforigin");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("AuthorId")
                        .HasName("pk_authors");

                    b.ToTable("authors");
                });

            modelBuilder.Entity("api_flms_service.Entity.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("bookid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BookId"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("authorid");

                    b.Property<int>("AvailableCopies")
                        .HasColumnType("integer")
                        .HasColumnName("availablecopies");

                    b.Property<string>("BookDescription")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("bookdescription");

                    b.Property<DateTime>("BorrowedUntil")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("borroweduntil");

                    b.Property<string>("CloudinaryImageId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cloudinaryimageid");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("isbn");

                    b.Property<string>("ImageUrls")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("imageurls");

                    b.Property<int>("PublicationYear")
                        .HasColumnType("integer")
                        .HasColumnName("publicationyear");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.HasKey("BookId")
                        .HasName("pk_books");

                    b.HasIndex("AuthorId");

                    b.ToTable("books");
                });

            modelBuilder.Entity("api_flms_service.Entity.BookCategory", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("integer")
                        .HasColumnName("bookid");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("categoryid");

                    b.Property<int?>("BookId1")
                        .HasColumnType("integer")
                        .HasColumnName("bookid1");

                    b.Property<int?>("CategoryId1")
                        .HasColumnType("integer")
                        .HasColumnName("categoryid1");

                    b.HasKey("BookId", "CategoryId")
                        .HasName("pk_bookcategory");

                    b.HasIndex("BookId1");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CategoryId1");

                    b.ToTable("bookcategory", (string)null);
                });

            modelBuilder.Entity("api_flms_service.Entity.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("categoryid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("categoryname");

                    b.HasKey("CategoryId")
                        .HasName("pk_categories");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("api_flms_service.Entity.IssuedBook", b =>
                {
                    b.Property<int>("SNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("sno");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SNo"));

                    b.Property<string>("BookAuthor")
                        .HasColumnType("text")
                        .HasColumnName("bookauthor");

                    b.Property<string>("BookName")
                        .HasColumnType("text")
                        .HasColumnName("bookname");

                    b.Property<string>("BookNo")
                        .HasColumnType("text")
                        .HasColumnName("bookno");

                    b.Property<DateTime?>("IssueDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("issuedate");

                    b.Property<string>("Status")
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer")
                        .HasColumnName("studentid");

                    b.HasKey("SNo")
                        .HasName("pk_issuedbooks");

                    b.ToTable("issuedbooks");
                });

            modelBuilder.Entity("api_flms_service.Entity.Loan", b =>
                {
                    b.Property<int>("BookLoanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("bookloanid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BookLoanId"));

                    b.Property<int>("BookId")
                        .HasColumnType("integer")
                        .HasColumnName("bookid");

                    b.Property<DateTime?>("LoanDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("loandate");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("returndate");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.HasKey("BookLoanId")
                        .HasName("pk_loans");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("loans");
                });

            modelBuilder.Entity("api_flms_service.Entity.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("reviewid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReviewId"));

                    b.Property<int>("BookId")
                        .HasColumnType("integer")
                        .HasColumnName("bookid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer")
                        .HasColumnName("rating");

                    b.Property<DateTime?>("ReviewDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("reviewdate");

                    b.Property<string>("ReviewText")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reviewtext");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.Property<int?>("UserId1")
                        .HasColumnType("integer")
                        .HasColumnName("userid1");

                    b.HasKey("ReviewId")
                        .HasName("pk_reviews");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("reviews");
                });

            modelBuilder.Entity("api_flms_service.Entity.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phonenumber");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("registrationdate");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("api_flms_service.Entity.Book", b =>
                {
                    b.HasOne("api_flms_service.Entity.Author", "Author")
                        .WithMany("Books")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("api_flms_service.Entity.BookCategory", b =>
                {
                    b.HasOne("api_flms_service.Entity.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Entity.Book", null)
                        .WithMany("BookCategories")
                        .HasForeignKey("BookId1");

                    b.HasOne("api_flms_service.Entity.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Entity.Category", null)
                        .WithMany("BookCategories")
                        .HasForeignKey("CategoryId1");

                    b.Navigation("Book");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("api_flms_service.Entity.Loan", b =>
                {
                    b.HasOne("api_flms_service.Entity.Book", "Book")
                        .WithMany("BookLoans")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Entity.User", "User")
                        .WithMany("BookLoans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api_flms_service.Entity.Review", b =>
                {
                    b.HasOne("api_flms_service.Entity.Book", "Book")
                        .WithMany("Reviews")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Entity.User", null)
                        .WithMany("BookReviews")
                        .HasForeignKey("UserId1");

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api_flms_service.Entity.Author", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("api_flms_service.Entity.Book", b =>
                {
                    b.Navigation("BookCategories");

                    b.Navigation("BookLoans");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("api_flms_service.Entity.Category", b =>
                {
                    b.Navigation("BookCategories");
                });

            modelBuilder.Entity("api_flms_service.Entity.User", b =>
                {
                    b.Navigation("BookLoans");

                    b.Navigation("BookReviews");
                });
#pragma warning restore 612, 618
        }
    }
}
