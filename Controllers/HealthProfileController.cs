using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DoAnChamSocSucKhoe.Data;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize]
    public class HealthProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthProfileController(ApplicationDbContext context)
        {
            _context = context;
        }        public async Task<IActionResult> Index()
        {
            // Redirect to the better UI in Patient area if authorized
            if (User.IsInRole("Patient") || User.HasClaim(c => c.Type == "CanViewProfile" && c.Value == "true"))
            {
                return RedirectToAction("Index", "HealthProfile", new { area = "Patient" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return NotFound();

            string targetUserId = userId!;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            HoSoSucKhoe? hoSoSucKhoe = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            if (hoSoSucKhoe == null)
            {
                hoSoSucKhoe = new HoSoSucKhoe { NguoiDungId = targetUserId };
                // Only add if it's the user's own profile? Or should we create one for the patient?
                // If we are viewing a patient, maybe we shouldn't auto-create here unless we are sure.
                // But keeping existing logic:
                _context.HoSoSucKhoes.Add(hoSoSucKhoe);
                await _context.SaveChangesAsync();
            }

            return View(hoSoSucKhoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HoSoSucKhoe model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return NotFound();
                string targetUserId = userId!;
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
                if (linkedPatient != null && linkedPatient.BenhNhanId != null) targetUserId = linkedPatient.BenhNhanId;

                // Ensure we are updating the correct profile
                model.NguoiDungId = targetUserId;

                var hoSoSucKhoe = await _context.HoSoSucKhoes
                    .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

                if (hoSoSucKhoe == null)
                {
                    return NotFound();
                }                
                
                // Lưu lịch sử sức khỏe
                var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.Id == targetUserId);
                if (nguoiDung == null)
                {
                    return NotFound();
                }
                
                                var lichSuSucKhoe = new LichSuSucKhoe
                                {
                                    NguoiDungId = targetUserId,
                                    NguoiDung = nguoiDung,
                                    NgayDo = System.DateTime.Now,
                                    CanNang = model.CanNang,
                                    ChieuCao = model.ChieuCao,
                                    DuongHuyet = model.DuongHuyet,
                                    HuyetApTamThu = model.HuyetApTamThu,
                                    HuyetApTamTruong = model.HuyetApTamTruong,
                                    NhipTim = model.NhipTim,
                                    GhiChu = model.GhiChu ?? string.Empty
                                };

                _context.LichSuSucKhoes.Add(lichSuSucKhoe);

                // Cập nhật hồ sơ sức khỏe
                hoSoSucKhoe.CanNang = model.CanNang;
                hoSoSucKhoe.ChieuCao = model.ChieuCao;
                hoSoSucKhoe.DuongHuyet = model.DuongHuyet;
                hoSoSucKhoe.HuyetApTamThu = model.HuyetApTamThu;
                hoSoSucKhoe.HuyetApTamTruong = model.HuyetApTamTruong;
                hoSoSucKhoe.NhipTim = model.NhipTim;
                hoSoSucKhoe.GhiChu = model.GhiChu;

                _context.Update(hoSoSucKhoe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Cập nhật thông tin sức khỏe thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> History()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return NotFound();

            string targetUserId = userId!;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId!;
            }

            var lichSuSucKhoe = await _context.LichSuSucKhoes
                .Where(l => l.NguoiDungId == targetUserId)
                .OrderByDescending(l => l.NgayDo)
                .ToListAsync();

            return View(lichSuSucKhoe);
        }
    }
}