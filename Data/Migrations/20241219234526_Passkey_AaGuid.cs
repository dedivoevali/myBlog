using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Passkey_AaGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AaGuid",
                table: "Passkey",
                type: "VARCHAR(900)",
                maxLength: 900,
                nullable: false,
                comment: "Specifies authenticator type (e.g Google Password Manager, iCloud Keychain etc)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AaGuid",
                table: "Passkey",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(900)",
                oldMaxLength: 900,
                oldComment: "Specifies authenticator type (e.g Google Password Manager, iCloud Keychain etc)");
        }
    }
}
