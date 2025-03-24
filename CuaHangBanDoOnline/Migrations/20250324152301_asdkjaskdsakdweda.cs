using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class asdkjaskdsakdweda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_HangHoas_HangHoaMaHangHoa",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_NguoiDungs_NguoiDungMaNguoiDung",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Wishlists_WishlistMaWishlist",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_HangHoaMaHangHoa",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_NguoiDungMaNguoiDung",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_WishlistMaWishlist",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "HangHoaMaHangHoa",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "NguoiDungMaNguoiDung",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "WishlistMaWishlist",
                table: "Wishlists");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_MaHangHoa",
                table: "Wishlists",
                column: "MaHangHoa");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_MaNguoiDung",
                table: "Wishlists",
                column: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_HangHoas_MaHangHoa",
                table: "Wishlists",
                column: "MaHangHoa",
                principalTable: "HangHoas",
                principalColumn: "MaHangHoa",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_NguoiDungs_MaNguoiDung",
                table: "Wishlists",
                column: "MaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_HangHoas_MaHangHoa",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_NguoiDungs_MaNguoiDung",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_MaHangHoa",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_MaNguoiDung",
                table: "Wishlists");

            migrationBuilder.AddColumn<int>(
                name: "HangHoaMaHangHoa",
                table: "Wishlists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NguoiDungMaNguoiDung",
                table: "Wishlists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WishlistMaWishlist",
                table: "Wishlists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_HangHoaMaHangHoa",
                table: "Wishlists",
                column: "HangHoaMaHangHoa");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_NguoiDungMaNguoiDung",
                table: "Wishlists",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_WishlistMaWishlist",
                table: "Wishlists",
                column: "WishlistMaWishlist");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_HangHoas_HangHoaMaHangHoa",
                table: "Wishlists",
                column: "HangHoaMaHangHoa",
                principalTable: "HangHoas",
                principalColumn: "MaHangHoa",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_NguoiDungs_NguoiDungMaNguoiDung",
                table: "Wishlists",
                column: "NguoiDungMaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Wishlists_WishlistMaWishlist",
                table: "Wishlists",
                column: "WishlistMaWishlist",
                principalTable: "Wishlists",
                principalColumn: "MaWishlist");
        }
    }
}
