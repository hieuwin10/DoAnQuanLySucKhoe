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
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hoSoSucKhoe = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == int.Parse(userId));

            if (hoSoSucKhoe == null)
            {
                hoSoSucKhoe = new HoSoSucKhoe { NguoiDungId = int.Parse(userId) };
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
                var hoSoSucKhoe = await _context.HoSoSucKhoes
                    .FirstOrDefaultAsync(h => h.NguoiDungId == model.NguoiDungId);

                if (hoSoSucKhoe == null)
                {
                    return NotFound();
                }

                // Lưu lịch sử sức khỏe
                var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.NguoiDungId == model.NguoiDungId);
                                if (nguoiDung == null)
                                {
                                    return NotFound();
                                }
                
                                var lichSuSucKhoe = new LichSuSucKhoe
                                {
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
            var lichSuSucKhoe = await _context.LichSuSucKhoes
                .Where(l => l.NguoiDungId == int.Parse(userId))
                .OrderByDescending(l => l.NgayDo)
                .ToListAsync();

            return View(lichSuSucKhoe);
        }
    }
} 