using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "BacSi,ChuyenGia")] // Ensure only doctors can access
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the ChuyenGia profile for this user
            // Note: ChuyenGiaId might be the same as UserId or different depending on your schema.
            // Based on previous files, ChuyenGia has NguoiDungId.
            // And LichHen has ChuyenGiaId.
            // We need to check if LichHen.ChuyenGiaId refers to the User ID or the ChuyenGia ID.
            // Looking at AppointmentController.cs: 
            // id = c.NguoiDungId (in GetDoctors)
            // So LichHen.ChuyenGiaId likely stores the NguoiDungId of the doctor.

            var appointments = await _context.LichHens
                .Include(a => a.NguoiDung) // Include Patient info
                .Where(a => a.ChuyenGiaId == userId)
                .OrderByDescending(a => a.NgayGioHen)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Doctor/Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lichHen = await _context.LichHens
                .Include(l => l.NguoiDung)
                .Include(l => l.NguoiDung.HoSoSucKhoe) // Include health profile if needed
                .FirstOrDefaultAsync(m => m.LichHenId == id);

            if (lichHen == null)
            {
                return NotFound();
            }

            // Verify access
            if (lichHen.ChuyenGiaId != userId)
            {
                return Unauthorized();
            }

            return View(lichHen);
        }

        public async Task<IActionResult> Pending()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.LichHens
                .Include(a => a.NguoiDung)
                .Where(a => a.ChuyenGiaId == userId && a.TrangThai == "Chờ xác nhận")
                .OrderBy(a => a.NgayGioHen)
                .ToListAsync();
            return View("Index", appointments);
        }

        public async Task<IActionResult> Confirmed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.LichHens
                .Include(a => a.NguoiDung)
                .Where(a => a.ChuyenGiaId == userId && a.TrangThai == "Đã xác nhận")
                .OrderBy(a => a.NgayGioHen)
                .ToListAsync();
            return View("Index", appointments);
        }

        public async Task<IActionResult> Completed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.LichHens
                .Include(a => a.NguoiDung)
                .Where(a => a.ChuyenGiaId == userId && a.TrangThai == "Đã hoàn thành")
                .OrderByDescending(a => a.NgayGioHen)
                .ToListAsync();
            return View("Index", appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _context.LichHens.FindAsync(id);
            if (appointment != null)
            {
                // Verify this appointment belongs to the logged-in doctor
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (appointment.ChuyenGiaId == userId)
                {
                    appointment.TrangThai = "Đã xác nhận";
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy lịch hẹn hoặc bạn không có quyền." });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var appointment = await _context.LichHens.FindAsync(id);
            if (appointment != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (appointment.ChuyenGiaId == userId)
                {
                    appointment.TrangThai = "Đã hủy";
                    appointment.LyDo = reason; // Or append to notes
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy lịch hẹn hoặc bạn không có quyền." });
        }
        
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var appointment = await _context.LichHens.FindAsync(id);
            if (appointment != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (appointment.ChuyenGiaId == userId)
                {
                    appointment.TrangThai = "Đã hoàn thành";
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy lịch hẹn hoặc bạn không có quyền." });
        }
    }
}
