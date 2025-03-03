using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class HoadonController : Controller
    {
        private readonly IHoadonRepository _repository;

        public HoadonController(IHoadonRepository repository)
        {
            _repository = repository;
        }

        // Hiển thị danh sách hóa đơn
        public IActionResult Index()
        {
            var hoadons = _repository.GetHoadons();
            return View(hoadons);
        }

        // Xem chi tiết hóa đơn
        public IActionResult Details(int id)
        {
            var hoadon = _repository.GetHoadon(id);
            if (hoadon == null) return NotFound();
            return View(hoadon);
        }

        // Form thêm hóa đơn
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(HoaDon hoadon)
        {
            if (ModelState.IsValid)
            {
                _repository.AddHoadon(hoadon);
                return RedirectToAction("Index");
            }
            return View(hoadon);
        }

        // Form chỉnh sửa hóa đơn
        public IActionResult Edit(int id)
        {
            var hoadon = _repository.GetHoadon(id);
            if (hoadon == null) return NotFound();
            return View(hoadon);
        }

        [HttpPost]
        public IActionResult Edit(int id, HoaDon hoadon)
        {
            if (id != hoadon.MaHoaDon) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.UpdateHoadon(hoadon);
                return RedirectToAction("Index");
            }
            return View(hoadon);
        }

        // Xác nhận xóa hóa đơn
        public IActionResult Delete(int id)
        {
            var hoadon = _repository.GetHoadon(id);
            if (hoadon == null) return NotFound();
            return View(hoadon);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteHoadon(id);
            return RedirectToAction("Index");
        }
    }
}
