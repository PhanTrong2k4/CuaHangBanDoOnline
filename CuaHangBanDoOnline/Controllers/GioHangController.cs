using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangBanDoOnline.Controllers
{
    public class GioHangController : Controller
    {
        private readonly IGioHangRepository _gioHangRepository;
        private readonly IChiTietGioHangRepository _chiTietGioHangRepository;
        private readonly IHangHoaRepository _hangHoaRepository;
        private readonly IDonHangRepository _donHangRepository;
        private readonly IChitietdonhangRepository _chiTietDonHangRepository;

        public GioHangController(
            IGioHangRepository gioHangRepository,
            IChiTietGioHangRepository chiTietGioHangRepository,
            IHangHoaRepository hangHoaRepository,
            IDonHangRepository donHangRepository,
            IChitietdonhangRepository chiTietDonHangRepository)
        {
            _gioHangRepository = gioHangRepository;
            _chiTietGioHangRepository = chiTietGioHangRepository;
            _hangHoaRepository = hangHoaRepository;
            _donHangRepository = donHangRepository;
            _chiTietDonHangRepository = chiTietDonHangRepository;
        }

        // Xem giỏ hàng
        public IActionResult Index()
        {
            try
            {
                int maNguoiDung = 1;
                var gioHang = _gioHangRepository.GetGioHangByUserId(maNguoiDung);
                if (gioHang == null || !gioHang.ChiTietGioHangs.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                    gioHang = new GioHang
                    {
                        MaNguoiDung = maNguoiDung,
                        ChiTietGioHangs = new List<ChiTietGioHang>()
                    };
                }
                return View(gioHang);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GioHangController.Index: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi tải giỏ hàng. Vui lòng thử lại.";
                return View(new GioHang
                {
                    MaNguoiDung = 1,
                    ChiTietGioHangs = new List<ChiTietGioHang>()
                });
            }
        }

        // Thêm sản phẩm vào giỏ hàng (cập nhật để hỗ trợ redirectToCheckout)
        [HttpPost]
        public IActionResult AddToCart(int id, int soLuong = 1, bool redirectToCheckout = false)
        {
            try
            {
                int maNguoiDung = 1;
                var gioHang = _gioHangRepository.GetGioHangByUserId(maNguoiDung);

                if (gioHang == null)
                {
                    gioHang = _gioHangRepository.CreateGioHang(maNguoiDung);
                }

                var chiTietGioHang = _chiTietGioHangRepository.GetChiTietGioHangByHangHoaId(gioHang.MaGioHang, id);
                if (chiTietGioHang != null)
                {
                    chiTietGioHang.SoLuong += soLuong;
                    _chiTietGioHangRepository.UpdateChiTietGioHang(chiTietGioHang);
                }
                else
                {
                    var hangHoa = _hangHoaRepository.GetHangHoa(id);
                    if (hangHoa == null)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                        }
                        TempData["Error"] = "Sản phẩm không tồn tại.";
                        return RedirectToAction("Index");
                    }

                    if (hangHoa.SoLuongTon < soLuong)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Số lượng tồn kho không đủ." });
                        }
                        TempData["Error"] = "Số lượng tồn kho không đủ.";
                        return RedirectToAction("Index");
                    }

                    chiTietGioHang = new ChiTietGioHang
                    {
                        MaGioHang = gioHang.MaGioHang,
                        MaHangHoa = id,
                        SoLuong = soLuong,
                        GiaBan = hangHoa.GiaBan
                    };
                    _chiTietGioHangRepository.AddChiTietGioHang(chiTietGioHang);
                }

                // Kiểm tra nếu yêu cầu là AJAX (từ JavaScript)
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng." });
                }

                // Nếu không phải AJAX (từ form), chuyển hướng dựa trên redirectToCheckout
                if (redirectToCheckout)
                {
                    return RedirectToAction("Checkout");
                }
                else
                {
                    TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong AddToCart: {ex.Message}");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng: " + ex.Message });
                }
                TempData["Error"] = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Index");
            }
        }

        // Lấy số lượng sản phẩm trong giỏ hàng
        [HttpGet]
        public IActionResult GetCartCount()
        {
            try
            {
                int maNguoiDung = 1;
                int count = _gioHangRepository.GetCartCountByUserId(maNguoiDung);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GetCartCount: {ex.Message}");
                return Json(new { count = 0 });
            }
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateCart(int maChiTietGioHang, int soLuong)
        {
            try
            {
                var chiTietGioHang = _chiTietGioHangRepository.GetChiTietGioHang(maChiTietGioHang);
                if (chiTietGioHang == null)
                {
                    TempData["Error"] = "Mục không tồn tại trong giỏ hàng.";
                    return RedirectToAction("Index");
                }

                if (soLuong <= 0)
                {
                    _chiTietGioHangRepository.DeleteChiTietGioHang(maChiTietGioHang);
                }
                else
                {
                    var hangHoa = _hangHoaRepository.GetHangHoa(chiTietGioHang.MaHangHoa);
                    if (hangHoa.SoLuongTon < soLuong)
                    {
                        TempData["Error"] = "Số lượng tồn kho không đủ.";
                        return RedirectToAction("Index");
                    }

                    chiTietGioHang.SoLuong = soLuong;
                    _chiTietGioHangRepository.UpdateChiTietGioHang(chiTietGioHang);
                }

                TempData["Success"] = "Đã cập nhật giỏ hàng.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong UpdateCart: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật giỏ hàng.";
                return RedirectToAction("Index");
            }
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int maChiTietGioHang)
        {
            try
            {
                var chiTietGioHang = _chiTietGioHangRepository.GetChiTietGioHang(maChiTietGioHang);
                if (chiTietGioHang == null)
                {
                    TempData["Error"] = "Mục không tồn tại trong giỏ hàng.";
                    return RedirectToAction("Index");
                }

                _chiTietGioHangRepository.DeleteChiTietGioHang(maChiTietGioHang);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong RemoveFromCart: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng.";
                return RedirectToAction("Index");
            }
        }

        // Chuyển giỏ hàng thành đơn hàng
        public IActionResult Checkout()
        {
            try
            {
                int maNguoiDung = 1;
                var gioHang = _gioHangRepository.GetGioHangByUserId(maNguoiDung);

                if (gioHang == null || !gioHang.ChiTietGioHangs.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra số lượng tồn kho
                foreach (var item in gioHang.ChiTietGioHangs)
                {
                    var hangHoa = _hangHoaRepository.GetHangHoa(item.MaHangHoa);
                    if (hangHoa.SoLuongTon < item.SoLuong)
                    {
                        TempData["Error"] = $"Sản phẩm {hangHoa.TenHangHoa} không đủ số lượng tồn kho.";
                        return RedirectToAction("Index");
                    }
                }

                // Tạo đơn hàng mới
                var donHang = new DonHang
                {
                    MaNguoiDung = maNguoiDung,
                    NgayDatHang = DateTime.Now,
                    TrangThai = "ChoDuyet",
                    ChiTietDonHangs = new List<ChiTietDonHang>()
                };

                // Thêm chi tiết đơn hàng
                foreach (var item in gioHang.ChiTietGioHangs)
                {
                    var chiTietDonHang = new ChiTietDonHang
                    {
                        MaHangHoa = item.MaHangHoa,
                        SoLuong = item.SoLuong,
                        GiaBan = item.GiaBan
                    };
                    donHang.ChiTietDonHangs.Add(chiTietDonHang);

                    // Cập nhật số lượng tồn kho
                    var hangHoa = _hangHoaRepository.GetHangHoa(item.MaHangHoa);
                    hangHoa.SoLuongTon -= item.SoLuong;
                    _hangHoaRepository.UpdateHangHoa(hangHoa);
                }

                _donHangRepository.CreateDonHang(donHang);

                // Xóa giỏ hàng sau khi tạo đơn hàng
                _gioHangRepository.DeleteGioHang(gioHang.MaGioHang);

                TempData["Success"] = "Đơn hàng đã được tạo thành công.";
                return RedirectToAction("Index", "DonHang");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong Checkout: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi tạo đơn hàng.";
                return RedirectToAction("Index");
            }
        }
    }
}