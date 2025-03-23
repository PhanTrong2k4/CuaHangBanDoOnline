using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class tongtien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "GioHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "DonHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "GioHangs");

            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "DonHangs");
        }
    }
}
