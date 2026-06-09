using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookRight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCombinedBookingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CombinedBookingId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombinedBookingId",
                table: "Appointments");
        }
    }
}
