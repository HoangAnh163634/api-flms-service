using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_flms_service.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowedUntilToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "borroweduntil",
                table: "books",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "userid",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "borroweduntil",
                table: "books");

            migrationBuilder.DropColumn(
                name: "userid",
                table: "books");
        }
    }
}
