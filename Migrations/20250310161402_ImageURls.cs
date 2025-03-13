using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_flms_service.Migrations
{
    /// <inheritdoc />
    public partial class ImageURls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageurls",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageurls",
                table: "books");
        }
    }
}
