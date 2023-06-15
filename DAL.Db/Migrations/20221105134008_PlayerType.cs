using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    public partial class PlayerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Type",
                table: "GameStates");

            migrationBuilder.DropColumn(
                name: "Player2Type",
                table: "GameStates");

            migrationBuilder.AddColumn<int>(
                name: "Player1Type",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Type",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Type",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Player2Type",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "Player1Type",
                table: "GameStates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Type",
                table: "GameStates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
