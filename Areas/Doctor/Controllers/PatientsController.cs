using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class PatientsController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;

        public PatientsController(UserManager<NguoiDung> userManager)
        {
            _userManager = userManager;
        }

        // GET: Doctor/Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients.ToList());
        }
    }
}
