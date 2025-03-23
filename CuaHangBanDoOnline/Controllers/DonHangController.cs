using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CuaHangBanDoOnline.Controllers
{
    
    public class DonHangController : Controller
    {
        private readonly IDonHangRepository _donHangRepository;

        public DonHangController(IDonHangRepository donHangRepository)
        {
            _donHangRepository = donHangRepository;
        }

        public IActionResult Index()
        {
            int maNguoiDung = 1;
            var donHangs = _donHangRepository.GetDonHangsByUserId(maNguoiDung);
            return View(donHangs);
        }

        public IActionResult Details(int id)
        {
            var donHang = _donHangRepository.GetDonHang(id);
            if (donHang == null || donHang.MaNguoiDung != 1)
            {
                return NotFound();
            }
            return View(donHang);
        }
    }
}