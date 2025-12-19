using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Threading.Tasks;
using System;

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class QuickActionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuickActionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Patient/QuickActions/QuickCreateAppointment
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> QuickCreateAppointment(string? chuyenGiaId, string? lyDo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var appointment = new LichHen
            {
                NguoiDungId = userId,
                ChuyenGiaId = chuyenGiaId ?? string.Empty,
                NgayGioHen = DateTime.Now.AddMinutes(30),
                NgayHen = DateTime.Now,
                DiaDiem = "Tư vấn trực tuyến",
                LyDo = string.IsNullOrEmpty(lyDo) ? "Đặt lịch nhanh" : lyDo,
                TrangThai = "0"
            };

            _context.LichHens.Add(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = appointment.LichHenId });
        }

        // POST: Patient/QuickActions/QuickCreateConsultation
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> QuickCreateConsultation(string tieuDe, string noiDung, string? chuyenGiaId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            // Load the user and specialist to satisfy required properties
            var user = await _context.NguoiDungs.FindAsync(userId);
            var specialist = string.IsNullOrEmpty(chuyenGiaId) 
                ? await _context.ChuyenGias.FirstOrDefaultAsync() 
                : await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == chuyenGiaId);

            if (user == null || specialist == null)
            {
                return Json(new { success = false, message = "User or specialist not found" });
            }

            var tuVan = new TuVanSucKhoe
            {
                NguoiDungId = userId,
                ChuyenGiaId = specialist.ChuyenGiaId,
                TieuDe = string.IsNullOrWhiteSpace(tieuDe) ? "Tư vấn nhanh" : tieuDe,
                NoiDung = string.IsNullOrWhiteSpace(noiDung) ? "" : noiDung,
                TraLoi = string.Empty,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now,
                TrangThai = 0,
                NguoiDung = user,
                ChuyenGia = specialist
            };

            _context.TuVanSucKhoes.Add(tuVan);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = tuVan.TuVanSucKhoeId });
        }
    }
}
