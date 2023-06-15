using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    public partial class CanEatBackwards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasToCaptureAll",
                table: "GameSettings",
                newName: "CanEatBackwards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanEatBackwards",
                table: "GameSettings",
                newName: "HasToCaptureAll");
        }
    }
}
