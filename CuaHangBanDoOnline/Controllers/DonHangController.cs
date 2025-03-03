using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class DonHangController : Controller
    {
        private readonly IDonHangRepository _repository;

        public DonHangController(IDonHangRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var donHangs = _repository.GetDonHangs();
            return View(donHangs);
        }

        public IActionResult Details(int id)
        {
            var donHang = _repository.GetDonHang(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _repository.AddDonHang(donHang);
                return RedirectToAction("Index");
            }
            return View(donHang);
        }

        public IActionResult Edit(int id)
        {
            var donHang = _repository.GetDonHang(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [HttpPost]
        public IActionResult Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.UpdateDonHang(donHang);
                return RedirectToAction("Index");
            }
            return View(donHang);
        }

        public IActionResult Delete(int id)
        {
            var donHang = _repository.GetDonHang(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteDonHang(id);
            return RedirectToAction("Index");
        }
    }
}