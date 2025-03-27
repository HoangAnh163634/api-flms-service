using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_flms_service.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_books_bookid1",
                table: "bookcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_categories_categoryid1",
                table: "bookcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_users_userid",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_users_userid1",
                table: "reviews");

            migrationBuilder.DropIndex(
                name: "IX_reviews_userid1",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "userid1",
                table: "reviews");

            migrationBuilder.RenameColumn(
                name: "categoryid1",
                table: "bookcategory",
                newName: "categoryid2");

            migrationBuilder.RenameColumn(
                name: "bookid1",
                table: "bookcategory",
                newName: "bookid2");

            migrationBuilder.RenameIndex(
                name: "IX_bookcategory_categoryid1",
                table: "bookcategory",
                newName: "IX_bookcategory_categoryid2");

            migrationBuilder.RenameIndex(
                name: "IX_bookcategory_bookid1",
                table: "bookcategory",
                newName: "IX_bookcategory_bookid2");

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_books_bookid2",
                table: "bookcategory",
                column: "bookid2",
                principalTable: "books",
                principalColumn: "bookid");

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_categories_categoryid2",
                table: "bookcategory",
                column: "categoryid2",
                principalTable: "categories",
                principalColumn: "categoryid");

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_users_userid",
                table: "reviews",
                column: "userid",
                principalTable: "users",
                principalColumn: "userid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_books_bookid2",
                table: "bookcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_bookcategory_categories_categoryid2",
                table: "bookcategory");

            migrationBuilder.DropForeignKey(
                name: "fk_reviews_users_userid",
                table: "reviews");

            migrationBuilder.RenameColumn(
                name: "categoryid2",
                table: "bookcategory",
                newName: "categoryid1");

            migrationBuilder.RenameColumn(
                name: "bookid2",
                table: "bookcategory",
                newName: "bookid1");

            migrationBuilder.RenameIndex(
                name: "IX_bookcategory_categoryid2",
                table: "bookcategory",
                newName: "IX_bookcategory_categoryid1");

            migrationBuilder.RenameIndex(
                name: "IX_bookcategory_bookid2",
                table: "bookcategory",
                newName: "IX_bookcategory_bookid1");

            migrationBuilder.AddColumn<int>(
                name: "userid1",
                table: "reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_userid1",
                table: "reviews",
                column: "userid1");

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_books_bookid1",
                table: "bookcategory",
                column: "bookid1",
                principalTable: "books",
                principalColumn: "bookid");

            migrationBuilder.AddForeignKey(
                name: "FK_bookcategory_categories_categoryid1",
                table: "bookcategory",
                column: "categoryid1",
                principalTable: "categories",
                principalColumn: "categoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_users_userid",
                table: "reviews",
                column: "userid",
                principalTable: "users",
                principalColumn: "userid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_users_userid1",
                table: "reviews",
                column: "userid1",
                principalTable: "users",
                principalColumn: "userid");
        }
    }
}
