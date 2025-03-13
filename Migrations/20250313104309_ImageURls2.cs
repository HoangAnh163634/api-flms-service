using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_flms_service.Migrations
{
    /// <inheritdoc />
    public partial class ImageURls2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_books_Author_AuthorTempId",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "FK_books_books_BooksBookId",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "FK_books_categories_CategoriesCategoryId",
                table: "books");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropIndex(
                name: "IX_books_catid",
                table: "books");

            migrationBuilder.DropIndex(
                name: "IX_authors_categoriescategoryid",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "address",
                table: "users");

            migrationBuilder.DropColumn(
                name: "googleid",
                table: "users");

            migrationBuilder.DropColumn(
                name: "id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "mobile",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "catid",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "catname",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "AuthorTempId",
                table: "books");

            migrationBuilder.DropColumn(
                name: "BooksBookId",
                table: "books");

            migrationBuilder.DropColumn(
                name: "CategoriesCategoryId",
                table: "books");

            migrationBuilder.DropColumn(
                name: "bookname",
                table: "books");

            migrationBuilder.DropColumn(
                name: "bookno",
                table: "books");

            migrationBuilder.DropColumn(
                name: "bookprice",
                table: "books");

            migrationBuilder.DropColumn(
                name: "catid",
                table: "books");

            migrationBuilder.DropColumn(
                name: "authorname",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "booksbookid",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "categoriescategoryid",
                table: "authors");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "issuedbooks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "issuedate",
                table: "issuedbooks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "bookno",
                table: "issuedbooks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "bookname",
                table: "issuedbooks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "bookauthor",
                table: "issuedbooks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "bookcategory",
                columns: table => new
                {
                    booksbookid = table.Column<int>(type: "integer", nullable: false),
                    categoriescategoryid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookcategory", x => new { x.booksbookid, x.categoriescategoryid });
                    table.ForeignKey(
                        name: "FK_bookcategory_books_booksbookid",
                        column: x => x.booksbookid,
                        principalTable: "books",
                        principalColumn: "bookid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bookcategory_categories_categoriescategoryid",
                        column: x => x.categoriescategoryid,
                        principalTable: "categories",
                        principalColumn: "categoryid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookcategory_categoriescategoryid",
                table: "bookcategory",
                column: "categoriescategoryid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookcategory");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "googleid",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "mobile",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "issuedbooks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "issuedate",
                table: "issuedbooks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "bookno",
                table: "issuedbooks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "bookname",
                table: "issuedbooks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "bookauthor",
                table: "issuedbooks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "catid",
                table: "categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "catname",
                table: "categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AuthorTempId",
                table: "books",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BooksBookId",
                table: "books",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoriesCategoryId",
                table: "books",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bookname",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "bookno",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "bookprice",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "catid",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "authorname",
                table: "authors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "booksbookid",
                table: "authors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "categoriescategoryid",
                table: "authors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    TempId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_Author_TempId", x => x.TempId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_catid",
                table: "books",
                column: "catid");

            migrationBuilder.CreateIndex(
                name: "IX_authors_categoriescategoryid",
                table: "authors",
                column: "categoriescategoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_books_Author_AuthorTempId",
                table: "books",
                column: "AuthorTempId",
                principalTable: "Author",
                principalColumn: "TempId");

            migrationBuilder.AddForeignKey(
                name: "FK_books_books_BooksBookId",
                table: "books",
                column: "BooksBookId",
                principalTable: "books",
                principalColumn: "bookid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_books_categories_CategoriesCategoryId",
                table: "books",
                column: "CategoriesCategoryId",
                principalTable: "categories",
                principalColumn: "categoryid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
