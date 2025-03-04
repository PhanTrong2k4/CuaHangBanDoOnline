using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class chinhsuahoadon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_NguoiDungs_MaNguoiDung",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_DonHangs_MaDonHang",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_MaDonHang",
                table: "HoaDons");

            migrationBuilder.AddColumn<int>(
                name: "DonHangMaDonHang",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaNguoiDung",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_DonHangMaDonHang",
                table: "HoaDons",
                column: "DonHangMaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaNguoiDung",
                table: "HoaDons",
                column: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_NguoiDungs_MaNguoiDung",
                table: "DonHangs",
                column: "MaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_DonHangs_DonHangMaDonHang",
                table: "HoaDons",
                column: "DonHangMaDonHang",
                principalTable: "DonHangs",
                principalColumn: "MaDonHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NguoiDungs_MaNguoiDung",
                table: "HoaDons",
                column: "MaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_NguoiDungs_MaNguoiDung",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_DonHangs_DonHangMaDonHang",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NguoiDungs_MaNguoiDung",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_DonHangMaDonHang",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_MaNguoiDung",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "DonHangMaDonHang",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "MaNguoiDung",
                table: "HoaDons");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaDonHang",
                table: "HoaDons",
                column: "MaDonHang");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_NguoiDungs_MaNguoiDung",
                table: "DonHangs",
                column: "MaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_DonHangs_MaDonHang",
                table: "HoaDons",
                column: "MaDonHang",
                principalTable: "DonHangs",
                principalColumn: "MaDonHang",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
