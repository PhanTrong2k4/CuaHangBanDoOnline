using CuaHangBanDoOnline.Models;

public interface IWishlistRepository
{
    IEnumerable<Wishlist> GetWishlistByUserId(int maNguoiDung);
    Wishlist GetWishlistItem(int maNguoiDung, int maHangHoa);
    void AddWishlistItem(Wishlist wishlistItem);
    void RemoveWishlistItem(int maWishlist);
    int GetWishlistCountByUserId(int maNguoiDung);
}