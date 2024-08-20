using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CubeEnergy.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedForTransactionetc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitBalance",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Transactions",
                newName: "TransactionDate");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transactions",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitBalance",
                table: "Transactions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
