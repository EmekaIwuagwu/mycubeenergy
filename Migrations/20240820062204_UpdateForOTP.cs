using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CubeEnergy.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OTPs",
                newName: "ExpiryTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryTime",
                table: "OTPs",
                newName: "CreatedAt");
        }
    }
}
