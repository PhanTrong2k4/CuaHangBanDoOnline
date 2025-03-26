using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "NguoiDungs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_UserProfileId",
                table: "NguoiDungs",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDungs_UserProfiles_UserProfileId",
                table: "NguoiDungs",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDungs_UserProfiles_UserProfileId",
                table: "NguoiDungs");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDungs_UserProfileId",
                table: "NguoiDungs");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "NguoiDungs");
        }
    }
}
