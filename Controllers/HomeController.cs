using System.Diagnostics;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnChamSocSucKhoe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Landing()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index");
            }
            return View();        }
        
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Landing");
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Contact()
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
