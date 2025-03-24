using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CuaHangBanDoOnline.Controllers
{
    public class BaseController : Controller
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IGioHangRepository _gioHangRepository;
        private readonly IMemoryCache _cache;

        // Constructor nhận các dependency thông qua Dependency Injection
        public BaseController(IWishlistRepository wishlistRepository, IGioHangRepository gioHangRepository, IMemoryCache memoryCache)
        {
            _wishlistRepository = wishlistRepository;
            _gioHangRepository = gioHangRepository;
            _cache = memoryCache;
        }

        // Ghi đè phương thức OnActionExecuting để thiết lập dữ liệu cho navbar
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            int maNguoiDung = 1;

            int wishlistCount = _wishlistRepository.GetWishlistCountByUserId(maNguoiDung);
            Console.WriteLine($"WishlistCount for user {maNguoiDung}: {wishlistCount}");
            ViewBag.WishlistCount = wishlistCount;

            int cartCount = _gioHangRepository.GetCartCountByUserId(maNguoiDung);
            Console.WriteLine($"CartCount for user {maNguoiDung}: {cartCount}");
            ViewBag.CartCount = cartCount;

            base.OnActionExecuting(context);
        }
    }
}