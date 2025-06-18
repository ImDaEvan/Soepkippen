using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoepkipAPI.Migrations
{
    /// <inheritdoc />
    public partial class tashItemupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "longditude",
                table: "Trash",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "weather_timestamp",
                table: "Trash",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weather_timestamp",
                table: "Trash");

            migrationBuilder.AlterColumn<float>(
                name: "longditude",
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
