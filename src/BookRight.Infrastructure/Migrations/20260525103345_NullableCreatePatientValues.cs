using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookRight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableCreatePatientValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(name: "Note", table: "Patients", nullable: true,
                oldClrType: typeof(string), oldNullable: false);
        }
    }
}
