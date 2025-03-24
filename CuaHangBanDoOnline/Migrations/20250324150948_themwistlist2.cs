using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    /// <inheritdoc />
    public partial class themwistlist2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WishlistMaWishlist",
                table: "Wishlists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_WishlistMaWishlist",
                table: "Wishlists",
                column: "WishlistMaWishlist");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Wishlists_WishlistMaWishlist",
                table: "Wishlists",
                column: "WishlistMaWishlist",
                principalTable: "Wishlists",
                principalColumn: "MaWishlist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Wishlists_WishlistMaWishlist",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_WishlistMaWishlist",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "WishlistMaWishlist",
                table: "Wishlists");
        }
    }
}
