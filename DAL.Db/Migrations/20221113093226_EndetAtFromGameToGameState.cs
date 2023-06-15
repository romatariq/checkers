using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    public partial class EndetAtFromGameToGameState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "Games");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "GameStates",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "GameStates");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }
    }
}
