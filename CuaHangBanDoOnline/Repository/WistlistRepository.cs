using CuaHangBanDoOnline.Models;
using System;
using Microsoft.EntityFrameworkCore;
public class WishlistRepository : IWishlistRepository
{
    private readonly CuaHangDbContext _context;

    public WishlistRepository(CuaHangDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Wishlist> GetWishlistByUserId(int maNguoiDung)
    {
        return _context.Wishlists
            .Include(w => w.HangHoa)
            .Where(w => w.MaNguoiDung == maNguoiDung)
            .ToList();
    }

    public Wishlist GetWishlistItem(int maNguoiDung, int maHangHoa)
    {
        return _context.Wishlists
            .FirstOrDefault(w => w.MaNguoiDung == maNguoiDung && w.MaHangHoa == maHangHoa);
    }

    public void AddWishlistItem(Wishlist wishlistItem)
    {
        _context.Wishlists.Add(wishlistItem);
        _context.SaveChanges();
    }

    public void RemoveWishlistItem(int maWishlist)
    {
        var item = _context.Wishlists.Find(maWishlist);
        if (item != null)
        {
            _context.Wishlists.Remove(item);
            _context.SaveChanges();
        }
    }

    public int GetWishlistCountByUserId(int maNguoiDung)
    {
        return _context.Wishlists
            .Count(w => w.MaNguoiDung == maNguoiDung);
    }
}