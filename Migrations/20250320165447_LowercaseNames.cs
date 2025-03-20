using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_flms_service.Migrations
{
    /// <inheritdoc />
    public partial class LowercaseNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    authorid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    biography = table.Column<string>(type: "text", nullable: true),
                    cloudinaryid = table.Column<string>(type: "text", nullable: true),
                    countryoforigin = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_authors", x => x.authorid);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoryid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    categoryname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.categoryid);
                });

            migrationBuilder.CreateTable(
                name: "issuedbooks",
                columns: table => new
                {
                    sno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bookno = table.Column<string>(type: "text", nullable: true),
                    bookname = table.Column<string>(type: "text", nullable: true),
                    bookauthor = table.Column<string>(type: "text", nullable: true),
                    studentid = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    issuedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issuedbooks", x => x.sno);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    phonenumber = table.Column<string>(type: "text", nullable: false),
                    registrationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    bookid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authorid = table.Column<int>(type: "integer", nullable: false),
                    availablecopies = table.Column<int>(type: "integer", nullable: false),
                    bookdescription = table.Column<string>(type: "text", nullable: true),
                    cloudinaryimageid = table.Column<string>(type: "text", nullable: false),
                    isbn = table.Column<string>(type: "text", nullable: true),
                    publicationyear = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    borroweduntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    imageurls = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.bookid);
                    table.ForeignKey(
                        name: "FK_books_authors_authorid",
                        column: x => x.authorid,
                        principalTable: "authors",
                        principalColumn: "authorid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bookcategory",
                columns: table => new
                {
                    bookid = table.Column<int>(type: "integer", nullable: false),
                    categoryid = table.Column<int>(type: "integer", nullable: false),
                    bookid1 = table.Column<int>(type: "integer", nullable: true),
                    categoryid1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookcategory", x => new { x.bookid, x.categoryid });
                    table.ForeignKey(
                        name: "FK_bookcategory_books_bookid",
                        column: x => x.bookid,
                        principalTable: "books",
                        principalColumn: "bookid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bookcategory_books_bookid1",
                        column: x => x.bookid1,
                        principalTable: "books",
                        principalColumn: "bookid");
                    table.ForeignKey(
                        name: "FK_bookcategory_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "categoryid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bookcategory_categories_categoryid1",
                        column: x => x.categoryid1,
                        principalTable: "categories",
                        principalColumn: "categoryid");
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    bookloanid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bookid = table.Column<int>(type: "integer", nullable: false),
                    loandate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    returndate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loans", x => x.bookloanid);
                    table.ForeignKey(
                        name: "FK_loans_books_bookid",
                        column: x => x.bookid,
                        principalTable: "books",
                        principalColumn: "bookid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_loans_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    reviewid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bookid = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    reviewdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewtext = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    userid1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.reviewid);
                    table.ForeignKey(
                        name: "FK_reviews_books_bookid",
                        column: x => x.bookid,
                        principalTable: "books",
                        principalColumn: "bookid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_users_userid1",
                        column: x => x.userid1,
                        principalTable: "users",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookcategory_bookid1",
                table: "bookcategory",
                column: "bookid1");

            migrationBuilder.CreateIndex(
                name: "IX_bookcategory_categoryid",
                table: "bookcategory",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_bookcategory_categoryid1",
                table: "bookcategory",
                column: "categoryid1");

            migrationBuilder.CreateIndex(
                name: "IX_books_authorid",
                table: "books",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "IX_loans_bookid",
                table: "loans",
                column: "bookid");

            migrationBuilder.CreateIndex(
                name: "IX_loans_userid",
                table: "loans",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_bookid",
                table: "reviews",
                column: "bookid");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_userid",
                table: "reviews",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_userid1",
                table: "reviews",
                column: "userid1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookcategory");

            migrationBuilder.DropTable(
                name: "issuedbooks");

            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "authors");
        }
    }
}
