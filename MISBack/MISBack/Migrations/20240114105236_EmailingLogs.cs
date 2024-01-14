using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MISBack.Migrations
{
    /// <inheritdoc />
    public partial class EmailingLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EmailingLogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmailingLogs");
        }
    }
}
