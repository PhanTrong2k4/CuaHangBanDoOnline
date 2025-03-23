using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class Ả : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HangHoaMaHangHoa",
                table: "KhuyenMais",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenHangHoa",
                table: "HangHoas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "HangHoas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaGoc",
                table: "HangHoas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMais_HangHoaMaHangHoa",
                table: "KhuyenMais",
                column: "HangHoaMaHangHoa");

            migrationBuilder.AddForeignKey(
                name: "FK_KhuyenMais_HangHoas_HangHoaMaHangHoa",
                table: "KhuyenMais",
                column: "HangHoaMaHangHoa",
                principalTable: "HangHoas",
                principalColumn: "MaHangHoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhuyenMais_HangHoas_HangHoaMaHangHoa",
                table: "KhuyenMais");

            migrationBuilder.DropIndex(
                name: "IX_KhuyenMais_HangHoaMaHangHoa",
                table: "KhuyenMais");

            migrationBuilder.DropColumn(
                name: "HangHoaMaHangHoa",
                table: "KhuyenMais");

            migrationBuilder.DropColumn(
                name: "GiaGoc",
                table: "HangHoas");

            migrationBuilder.AlterColumn<string>(
                name: "TenHangHoa",
                table: "HangHoas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "HangHoas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
