using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ReminderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public ReminderController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Doctor/Reminder?patientId=...
        public async Task<IActionResult> Index(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                // Handle error or redirect: no patient specified
                return NotFound("Vui lòng cung cấp ID bệnh nhân.");
            }

            var patient = await _userManager.FindByIdAsync(patientId);
            if (patient == null)
            {
                return NotFound("Không tìm thấy bệnh nhân.");
            }

            var reminders = await _context.NhacNhoSucKhoes
                .Where(r => r.UserId == patientId)
                .OrderByDescending(r => r.ThoiGian)
                .ToListAsync();

            ViewData["PatientName"] = patient.HoTen;
            ViewData["PatientId"] = patient.Id;

            return View(reminders);
        }

        // POST: Doctor/Reminder/MarkAsCompleted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCompleted(int id, string patientId)
        {
            var reminder = await _context.NhacNhoSucKhoes.FindAsync(id);

            if (reminder == null || reminder.UserId != patientId)
            {
                return NotFound();
            }

            if (!reminder.DaThucHien)
            {
                reminder.DaThucHien = true;
                reminder.NgayCapNhat = System.DateTime.Now;
                _context.Update(reminder);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { patientId = patientId });
        }
    }
}
