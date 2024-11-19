using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class User_Initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Initials",
                table: "User",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                computedColumnSql: "CASE \r\n    WHEN [FirstName] IS NOT NULL AND [LastName] IS NOT NULL \r\n        THEN SUBSTRING([FirstName], 1, 1) + SUBSTRING([LastName], 1, 1)\r\n    ELSE SUBSTRING([Username], 1, 1)\r\nEND",
                stored: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Initials",
                table: "User");
        }
    }
}
