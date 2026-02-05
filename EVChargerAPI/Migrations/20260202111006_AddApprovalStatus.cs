using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVChargerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stations_Users_OwnerId",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_OwnerId",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Stations");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Stations");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Stations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_OwnerId",
                table: "Stations",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_Users_OwnerId",
                table: "Stations",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
