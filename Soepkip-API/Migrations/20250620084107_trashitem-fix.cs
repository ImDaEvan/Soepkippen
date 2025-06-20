using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoepkipAPI.Migrations
{
    /// <inheritdoc />
    public partial class trashitemfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "latitude",
                table: "Trash",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "latitude",
                table: "Trash",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float",
                oldNullable: true);
        }
    }
}
