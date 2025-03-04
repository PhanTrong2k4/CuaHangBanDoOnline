using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly IHangHoaRepository _repository;

        public HangHoaController(IHangHoaRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var hangHoas = _repository.GetHangHoas();
            return View(hangHoas);
        }

        public IActionResult Details(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            return View(hangHoa);
        }

        public IActionResult Create() 
        {
            ViewBag.DanhMucs = new MultiSelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(new HangHoa());
        }


        [HttpPost]
        public IActionResult Create(HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (ModelState.IsValid)
            {
                List<int> danhMucIds = Request.Form["DanhMucIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(int.Parse)
                    .ToList();

                _repository.AddHangHoa(hangHoa, danhMucIds, HinhAnh);
                return RedirectToAction("Index");
            }
            return View(hangHoa);
        }



        public IActionResult Edit(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            // Lấy danh sách tên danh mục hiện tại của sản phẩm
            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => hdm.DanhMuc.TenDanhMuc)
                .ToList();

            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<string>();

            // Lấy tất cả danh mục để thêm vào
            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");

            return View(hangHoa); // Không cần ViewBag.HinhAnhHienTai vì View đã dùng Model.Hinh trực tiếp
        }



        [HttpPost]
        public IActionResult Themdanhmuc(int maHangHoa, int maDanhMuc)
        {
            var hangHoa = _repository.GetHangHoa(maHangHoa);
            if (hangHoa == null) return NotFound();

            _repository.ThemDanhMuc(maHangHoa, maDanhMuc);
            return RedirectToAction("Edit", new { id = maHangHoa });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (id != hangHoa.MaHangHoa) return BadRequest();

            if (ModelState.IsValid)
            {
                // Lấy thông tin hàng hóa hiện tại từ repository để giữ các giá trị không thay đổi
                var existingHangHoa = _repository.GetHangHoa(id);
                if (existingHangHoa == null) return NotFound();

                // Cập nhật các thuộc tính từ form
                existingHangHoa.TenHangHoa = hangHoa.TenHangHoa;
                existingHangHoa.GiaBan = hangHoa.GiaBan;
                existingHangHoa.MoTa = hangHoa.MoTa;
                existingHangHoa.SoLuongTon = hangHoa.SoLuongTon;

                // Xử lý hình ảnh mới nếu có
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    // Xóa ảnh cũ nếu tồn tại (tùy chọn)
                    if (!string.IsNullOrEmpty(existingHangHoa.Hinh))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHangHoa.Hinh.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }
                    existingHangHoa.Hinh = "/images/" + fileName; // Cập nhật đường dẫn ảnh mới
                }
                // Nếu không có ảnh mới, giữ nguyên ảnh cũ (existingHangHoa.Hinh không bị thay đổi)

                _repository.UpdateHangHoa(existingHangHoa);
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, trả lại View với dữ liệu cần thiết
            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => hdm.DanhMuc.TenDanhMuc)
                .ToList();
            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<string>();
            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(hangHoa);
        }

        public IActionResult Delete(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            return View(hangHoa);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteHangHoa(id);
            return RedirectToAction("Index");
        }
    }
}
