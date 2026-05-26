using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookRight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameToBackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Clinics",
                table: "Practitioners",
                newName: "_clinics");

            migrationBuilder.RenameColumn(
                name: "Appointments",
                table: "Practitioners",
                newName: "_appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "_clinics",
                table: "Practitioners",
                newName: "Clinics");

            migrationBuilder.RenameColumn(
                name: "_appointments",
                table: "Practitioners",
                newName: "Appointments");
        }
    }
}
