using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;
using DoAnChamSocSucKhoe.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class HealthRecordsController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly ApplicationDbContext _context;

        public HealthRecordsController(UserManager<NguoiDung> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients.ToList());
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _userManager.FindByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new HealthRecordDetailViewModel
            {
                Patient = patient,
                HealthProfile = await _context.HoSoSucKhoes.FirstOrDefaultAsync(h => h.NguoiDungId == id),
                HealthMetrics = await _context.ChiSoSucKhoes.Where(c => c.NguoiDungId == id).OrderByDescending(c => c.NgayDo).ToListAsync(),
                HealthHistories = await _context.LichSuSucKhoes.Where(l => l.NguoiDungId == id).OrderByDescending(l => l.NgayDo).ToListAsync(),
                NutritionPlans = await _context.KeHoachDinhDuongs.Where(p => p.NguoiDungId == id).Include(p => p.ChiTietKeHoachDinhDuongs).OrderByDescending(p => p.NgayBatDau).ToListAsync(),
                ExercisePlans = await _context.KeHoachTapLuyens.Where(p => p.NguoiDungId == id).Include(p => p.ChiTietKeHoachTapLuyens).OrderByDescending(p => p.NgayBatDau).ToListAsync(),
                HealthReminders = await _context.NhacNhoSucKhoes.Where(r => r.UserId == id).OrderByDescending(r => r.ThoiGian).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReminder(string patientId, string noiDung, DateTime thoiGian)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(noiDung))
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            var reminder = new NhacNhoSucKhoe
            {
                UserId = patientId,
                NoiDung = noiDung,
                ThoiGian = thoiGian,
                LoaiNhacNho = "Uống thuốc", // Hardcoded for this case
                DaThucHien = false,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.NhacNhoSucKhoes.Add(reminder);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new { id = patientId });
        }
    }
}
