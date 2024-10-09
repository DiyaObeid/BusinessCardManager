using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCardManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhotoColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "BusinessCards",
                type: "NVARCHAR(MAX)", // Explicitly setting to NVARCHAR(MAX)
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)"); // The type you're changing from
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "BusinessCards",
                type: "NVARCHAR(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)"); // Changing back to NVARCHAR(500)
        }


    }
}
