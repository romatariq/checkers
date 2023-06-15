using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    public partial class RemovedMostCaptureSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasToJumpWithKingIfCaptureAmountIsSame",
                table: "GameSettings");

            migrationBuilder.DropColumn(
                name: "HasToTakeJumpWithMostEnemyButtonsCaptured",
                table: "GameSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasToJumpWithKingIfCaptureAmountIsSame",
                table: "GameSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasToTakeJumpWithMostEnemyButtonsCaptured",
                table: "GameSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
