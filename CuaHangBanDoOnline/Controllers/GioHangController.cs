using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Authorization;
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
            int maNguoiDung = 1; // Hardcode để test, thay bằng User.FindFirst khi dùng Identity
            var gioHang = _gioHangRepository.GetGioHangByUserId(maNguoiDung);
            if (gioHang == null || !gioHang.ChiTietGioHangs.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return View(new GioHang());
            }
            return View(gioHang);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpGet]
        public IActionResult AddToCart(int id, int soLuong = 1)
        {
            int maNguoiDung = 1; // Hardcode để test
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
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Index", "Home");
                }

                if (hangHoa.SoLuongTon < soLuong)
                {
                    TempData["Error"] = "Số lượng tồn kho không đủ.";
                    return RedirectToAction("Detail", "HangHoa", new { id });
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

            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateCart(int maChiTietGioHang, int soLuong)
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

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int maChiTietGioHang)
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

        // Chuyển giỏ hàng thành đơn hàng
        public IActionResult Checkout()
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
    }
}