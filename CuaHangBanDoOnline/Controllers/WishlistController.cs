using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CuaHangBanDoOnline.Controllers
{
    public class WishlistController : BaseController
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMemoryCache _cache;

        public WishlistController(IWishlistRepository wishlistRepository, IGioHangRepository gioHangRepository, IMemoryCache memoryCache)
            : base(wishlistRepository, gioHangRepository, memoryCache)
        {
            _wishlistRepository = wishlistRepository;
            _cache = memoryCache;
        }

        // Hiển thị danh sách wishlist của người dùng
        public IActionResult Index()
        {
            try
            {
                int maNguoiDung = 1; // Thay bằng ID của người dùng hiện tại (ví dụ: từ User.Identity)
                var wishlists = _wishlistRepository.GetWishlistByUserId(maNguoiDung);
                return View(wishlists);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong WishlistController.Index: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách yêu thích. Vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult CheckWishlistStatus(int id)
        {
            int maNguoiDung = 1;
            var existingItem = _wishlistRepository.GetWishlistItem(maNguoiDung, id);
            return Json(new { isInWishlist = existingItem != null });
        }

        [HttpPost]
        public IActionResult AddToWishlist(int id)
        {
            int maNguoiDung = 1;
            var existingItem = _wishlistRepository.GetWishlistItem(maNguoiDung, id);

            if (existingItem == null)
            {
                var wishlistItem = new Wishlist
                {
                    MaNguoiDung = maNguoiDung,
                    MaHangHoa = id
                };
                _wishlistRepository.AddWishlistItem(wishlistItem);
                _cache.Remove($"WishlistCount_{maNguoiDung}");
                return Json(new { success = true, message = "Đã thêm sản phẩm vào danh sách yêu thích." });
            }

            return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích." });
        }

        [HttpPost]
        public IActionResult RemoveFromWishlistByHangHoaId(int hangHoaId)
        {
            int maNguoiDung = 1;
            var wishlistItem = _wishlistRepository.GetWishlistItem(maNguoiDung, hangHoaId);
            if (wishlistItem != null)
            {
                _wishlistRepository.RemoveWishlistItem(wishlistItem.MaWishlist);
                _cache.Remove($"WishlistCount_{maNguoiDung}");
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi danh sách yêu thích." });
            }
            return Json(new { success = false, message = "Sản phẩm không có trong danh sách yêu thích." });
        }

        [HttpGet]
        public IActionResult GetWishlistCount()
        {
            int maNguoiDung = 1;
            var count = _wishlistRepository.GetWishlistCountByUserId(maNguoiDung);
            return Json(new { count });
        }
    }
}