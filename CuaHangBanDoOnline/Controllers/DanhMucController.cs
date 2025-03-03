using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly IDanhMucRepository _repository;

        public DanhMucController(IDanhMucRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var danhMucs = _repository.GetDanhMucs();
            return View(danhMucs);
        }

        public IActionResult Details(int id)
        {
            var danhMuc = _repository.GetDanhMuc(id);
            if (danhMuc == null) return NotFound();
            return View(danhMuc);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                _repository.AddDanhMuc(danhMuc);
                return RedirectToAction("Index");
            }
            return View(danhMuc);
        }

        public IActionResult Edit(int id)
        {
            var danhMuc = _repository.GetDanhMuc(id);
            if (danhMuc == null) return NotFound();
            return View(danhMuc);
        }

        [HttpPost]
        public IActionResult Edit(int id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.UpdateDanhMuc(danhMuc);
                return RedirectToAction("Index");
            }
            return View(danhMuc);
        }

        public IActionResult Delete(int id)
        {
            var danhMuc = _repository.GetDanhMuc(id);
            if (danhMuc == null) return NotFound();
            return View(danhMuc);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteDanhMuc(id);
            return RedirectToAction("Index");
        }
    }
}