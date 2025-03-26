using CuaHangBanDoOnline.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CuaHangBanDoOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kiểm tra xem có JWToken trong Session không
            var token = HttpContext.Session.GetString("JWToken");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(token); // Truyền trạng thái qua ViewBag
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
