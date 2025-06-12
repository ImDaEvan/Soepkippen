using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoepkipAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Trash",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    confidence = table.Column<float>(type: "float", nullable: false),
                    longditude = table.Column<float>(type: "float", nullable: false),
                    latitude = table.Column<float>(type: "float", nullable: false),
                    feels_like_temp_celsius = table.Column<float>(type: "float", nullable: true),
                    actual_temp_celsius = table.Column<float>(type: "float", nullable: true),
                    wind_force_bft = table.Column<float>(type: "float", nullable: true),
                    wind_direction = table.Column<float>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trash", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trash");
        }
    }
}
