using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly IThanhToanRepository _repository;

        public ThanhToanController(IThanhToanRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var thanhToans = _repository.GetThanhToans();
            return View(thanhToans);
        }

        public IActionResult Details(int id)
        {
            var thanhToan = _repository.GetThanhToan(id);
            if (thanhToan == null) return NotFound();
            return View(thanhToan);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ThanhToan thanhToan)
        {
            if (ModelState.IsValid)
            {
                _repository.AddThanhToan(thanhToan);
                return RedirectToAction("Index");
            }
            return View(thanhToan);
        }

        public IActionResult Edit(int id)
        {
            var thanhToan = _repository.GetThanhToan(id);
            if (thanhToan == null) return NotFound();
            return View(thanhToan);
        }

        [HttpPost]
        public IActionResult Edit(int id, ThanhToan thanhToan)
        {
            if (id != thanhToan.MaThanhToan) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.UpdateThanhToan(thanhToan);
                return RedirectToAction("Index");
            }
            return View(thanhToan);
        }

        public IActionResult Delete(int id)
        {
            var thanhToan = _repository.GetThanhToan(id);
            if (thanhToan == null) return NotFound();
            return View(thanhToan);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteThanhToan(id);
            return RedirectToAction("Index");
        }
    }
}
