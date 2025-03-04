using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class ADSADSA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HangHoas_DanhMucs_MaDanhMuc",
                table: "HangHoas");

            migrationBuilder.DropIndex(
                name: "IX_HangHoas_MaDanhMuc",
                table: "HangHoas");

            migrationBuilder.DropColumn(
                name: "MaDanhMuc",
                table: "HangHoas");

            migrationBuilder.AddColumn<string>(
                name: "Hinh",
                table: "HangHoas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HangHoaDanhMucs",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    MaHangHoa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangHoaDanhMucs", x => new { x.MaDanhMuc, x.MaHangHoa });
                    table.ForeignKey(
                        name: "FK_HangHoaDanhMucs_DanhMucs_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMucs",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HangHoaDanhMucs_HangHoas_MaHangHoa",
                        column: x => x.MaHangHoa,
                        principalTable: "HangHoas",
                        principalColumn: "MaHangHoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HangHoaDanhMucs_MaHangHoa",
                table: "HangHoaDanhMucs",
                column: "MaHangHoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HangHoaDanhMucs");

            migrationBuilder.DropColumn(
                name: "Hinh",
                table: "HangHoas");

            migrationBuilder.AddColumn<int>(
                name: "MaDanhMuc",
                table: "HangHoas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HangHoas_MaDanhMuc",
                table: "HangHoas",
                column: "MaDanhMuc");

            migrationBuilder.AddForeignKey(
                name: "FK_HangHoas_DanhMucs_MaDanhMuc",
                table: "HangHoas",
                column: "MaDanhMuc",
                principalTable: "DanhMucs",
                principalColumn: "MaDanhMuc",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
