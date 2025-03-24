using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class addwistlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    MaWishlist = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaHangHoa = table.Column<int>(type: "int", nullable: false),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    HangHoaMaHangHoa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.MaWishlist);
                    table.ForeignKey(
                        name: "FK_Wishlists_HangHoas_HangHoaMaHangHoa",
                        column: x => x.HangHoaMaHangHoa,
                        principalTable: "HangHoas",
                        principalColumn: "MaHangHoa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_HangHoaMaHangHoa",
                table: "Wishlists",
                column: "HangHoaMaHangHoa");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_NguoiDungMaNguoiDung",
                table: "Wishlists",
                column: "NguoiDungMaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wishlists");
        }
    }
}
