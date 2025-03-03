using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(HangHoa hangHoa)
        {
            if (ModelState.IsValid)
            {
                _repository.AddHangHoa(hangHoa);
                return RedirectToAction("Index");
            }
            return View(hangHoa);
        }

        public IActionResult Edit(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            return View(hangHoa);
        }

        [HttpPost]
        public IActionResult Edit(int id, HangHoa hangHoa)
        {
            if (id != hangHoa.MaHangHoa) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.UpdateHangHoa(hangHoa);
                return RedirectToAction("Index");
            }
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
