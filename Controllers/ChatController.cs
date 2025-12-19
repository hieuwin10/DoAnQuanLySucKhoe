using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize] // Only logged-in users can access the chat
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
