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
            migrationBuilder.AddColumn<decimal>(
                name: "PhanTramGiamGia",
                table: "DanhMucs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhanTramGiamGia",
                table: "DanhMucs");
        }
    }
}
