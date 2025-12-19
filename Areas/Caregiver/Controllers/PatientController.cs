using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Caregiver.Controllers
{
    [Area("Caregiver")]
    [Authorize(Roles = "Caregiver")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public PatientController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Caregiver/Patient
        public async Task<IActionResult> Index()
        {
            var caregiverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (caregiverId == null) return Unauthorized();
            string currentCaregiverId = caregiverId;

#pragma warning disable CS8602 // Dereference of a possibly null reference
            var patients = await _context.NguoiChamSocBenhNhans
                .Where(cs => cs.NguoiChamSocId == currentCaregiverId && cs.BenhNhanId != null)
                .Include(cs => cs.BenhNhan)
                .ThenInclude(bn => bn.HoSoSucKhoe)
                .Select(cs => cs.BenhNhan!)
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference

            return View(patients);
        }

        // GET: Caregiver/Patient/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caregiverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (caregiverId == null) return Unauthorized();

            // Verify that the caregiver is assigned to this patient
            var isAuthorized = await _context.NguoiChamSocBenhNhans
                .AnyAsync(cs => cs.NguoiChamSocId == caregiverId && cs.BenhNhanId == id);

            if (!isAuthorized)
            {
                return Forbid();
            }

            var patient = await _context.Users
                .Include(u => u.HoSoSucKhoe)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null || patient.HoSoSucKhoe == null)
            {
                return NotFound();
            }

            return View(patient.HoSoSucKhoe);
        }
    }
}
