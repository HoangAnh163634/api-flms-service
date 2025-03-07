﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using api_flms_service.Model;

#nullable disable

namespace api_flms_service.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250217174644_LowercaseNames")]
    partial class LowercaseNames
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("api_flms_service.Model.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<long>("Mobile")
                        .HasColumnType("bigint")
                        .HasColumnName("mobile");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.HasKey("Id")
                        .HasName("pk_admins");

                    b.ToTable("admins");
                });

            modelBuilder.Entity("api_flms_service.Model.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("authorid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AuthorId"));

                    b.Property<string>("AuthorName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("authorname");

                    b.HasKey("AuthorId")
                        .HasName("pk_authors");

                    b.ToTable("authors");
                });

            modelBuilder.Entity("api_flms_service.Model.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("bookid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BookId"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("authorid");

                    b.Property<string>("BookName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("bookname");

                    b.Property<int>("BookNo")
                        .HasColumnType("integer")
                        .HasColumnName("bookno");

                    b.Property<int>("BookPrice")
                        .HasColumnType("integer")
                        .HasColumnName("bookprice");

                    b.Property<int>("CatId")
                        .HasColumnType("integer")
                        .HasColumnName("catid");

                    b.HasKey("BookId")
                        .HasName("pk_books");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CatId");

                    b.ToTable("books");
                });

            modelBuilder.Entity("api_flms_service.Model.Category", b =>
                {
                    b.Property<int>("CatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("catid");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CatId"));

                    b.Property<string>("CatName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("catname");

                    b.HasKey("CatId")
                        .HasName("pk_categories");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("api_flms_service.Model.IssuedBook", b =>
                {
                    b.Property<int>("SNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("sno");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SNo"));

                    b.Property<string>("BookAuthor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("bookauthor");

                    b.Property<string>("BookName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("bookname");

                    b.Property<int>("BookNo")
                        .HasColumnType("integer")
                        .HasColumnName("bookno");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("issuedate");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer")
                        .HasColumnName("studentid");

                    b.HasKey("SNo")
                        .HasName("pk_issuedbooks");

                    b.ToTable("issuedbooks");
                });

            modelBuilder.Entity("api_flms_service.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<int?>("GoogleId")
                        .HasColumnType("integer")
                        .HasColumnName("googleid");

                    b.Property<long>("Mobile")
                        .HasColumnType("bigint")
                        .HasColumnName("mobile");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("api_flms_service.Model.Book", b =>
                {
                    b.HasOne("api_flms_service.Model.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api_flms_service.Model.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
