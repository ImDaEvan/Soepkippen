using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoepkipAPI.Migrations
{
    /// <inheritdoc />
    public partial class trashitemfix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longditude",
                table: "Trash",
                newName: "longitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Trash",
                newName: "longditude");
        }
    }
}
