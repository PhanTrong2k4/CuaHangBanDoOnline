using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CuaHangBanDoOnline.Controllers
{
    public class WishlistController : BaseController
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;

        public WishlistController(
            IWishlistRepository wishlistRepository,
            IGioHangRepository gioHangRepository,
            IMemoryCache memoryCache,
            IUserRepository userRepository)
            : base(wishlistRepository, gioHangRepository, memoryCache)
        {
            _wishlistRepository = wishlistRepository;
            _cache = memoryCache;
            _userRepository = userRepository;
        }

        // Hiển thị danh sách wishlist của người dùng
        public IActionResult Index()
        {
            try
            {
                var user = GetCurrentUser();
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var wishlists = _wishlistRepository.GetWishlistByUserId(user.MaNguoiDung);
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
            var user = GetCurrentUser();
            if (user == null)
            {
                return Json(new { isInWishlist = false });
            }

            var existingItem = _wishlistRepository.GetWishlistItem(user.MaNguoiDung, id);
            return Json(new { isInWishlist = existingItem != null });
        }

        [HttpPost]
        public IActionResult AddToWishlist(int id)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào danh sách yêu thích." });
            }

            var existingItem = _wishlistRepository.GetWishlistItem(user.MaNguoiDung, id);

            if (existingItem == null)
            {
                var wishlistItem = new Wishlist
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaHangHoa = id
                };
                _wishlistRepository.AddWishlistItem(wishlistItem);
                _cache.Remove($"WishlistCount_{user.MaNguoiDung}");
                return Json(new { success = true, message = "Đã thêm sản phẩm vào danh sách yêu thích." });
            }

            return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích." });
        }

        [HttpPost]
        public IActionResult RemoveFromWishlistByHangHoaId(int hangHoaId)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để xóa sản phẩm khỏi danh sách yêu thích." });
            }

            var wishlistItem = _wishlistRepository.GetWishlistItem(user.MaNguoiDung, hangHoaId);
            if (wishlistItem != null)
            {
                _wishlistRepository.RemoveWishlistItem(wishlistItem.MaWishlist);
                _cache.Remove($"WishlistCount_{user.MaNguoiDung}");
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi danh sách yêu thích." });
            }
            return Json(new { success = false, message = "Sản phẩm không có trong danh sách yêu thích." });
        }

        [HttpGet]
        public IActionResult GetWishlistCount()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return Json(new { count = 0 });
            }

            var count = _wishlistRepository.GetWishlistCountByUserId(user.MaNguoiDung);
            return Json(new { count });
        }

        // Phương thức tiện ích để lấy thông tin người dùng hiện tại từ token
        private User GetCurrentUser()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            return _userRepository.GetUserByEmailOrUsername(username);
        }
    }
}