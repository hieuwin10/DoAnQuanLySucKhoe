using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnChamSocSucKhoe.Areas.Caregiver.Controllers
{
    [Area("Caregiver")]
    [Authorize(Roles = "Caregiver")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
