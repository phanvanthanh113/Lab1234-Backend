using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class Lab7_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelResults_CustomUsers_userId1",
                table: "LevelResults");

            migrationBuilder.DropIndex(
                name: "IX_LevelResults_userId1",
                table: "LevelResults");

            migrationBuilder.DropColumn(
                name: "userId1",
                table: "LevelResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId1",
                table: "LevelResults",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LevelResults_userId1",
                table: "LevelResults",
                column: "userId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LevelResults_CustomUsers_userId1",
                table: "LevelResults",
                column: "userId1",
                principalTable: "CustomUsers",
                principalColumn: "userId");
        }
    }
}
