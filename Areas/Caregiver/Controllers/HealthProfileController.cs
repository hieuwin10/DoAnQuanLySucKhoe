using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Areas.Caregiver.Controllers
{
    [Area("Caregiver")]
    [Authorize(Roles = "Caregiver")]
    public class HealthProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public HealthProfileController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Caregiver/HealthProfile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string targetUserId = user.Id;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == user.Id);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId!;
            }

            var hoSo = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            if (hoSo == null)
            {
                hoSo = new HoSoSucKhoe
                {
                    NguoiDungId = targetUserId,
                    NgayCapNhat = DateTime.Now
                };
            }

            return View(hoSo);
        }

        // GET: Caregiver/HealthProfile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string targetUserId = user.Id;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == user.Id);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId!;
            }

            var hoSo = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            if (hoSo == null)
            {
                hoSo = new HoSoSucKhoe
                {
                    NguoiDungId = targetUserId,
                    NgayCapNhat = DateTime.Now
                };
            }

            return View(hoSo);
        }

        // POST: Caregiver/HealthProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HoSoSucKhoe hoSo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string targetUserId = user.Id;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == user.Id);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId!;
            }

            var existing = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            if (existing == null)
            {
                hoSo.NguoiDungId = targetUserId;
                hoSo.NgayCapNhat = DateTime.Now;
                _context.HoSoSucKhoes.Add(hoSo);
                
                var lichSu = new LichSuHoSoSucKhoe
                {
                    HoSoSucKhoeId = hoSo.HoSoSucKhoeId,
                    NguoiThayDoiId = user.Id,
                    NgayThayDoi = DateTime.Now,
                    LoaiThayDoi = "Tạo mới",
                    ThayDoiNoiDung = "Người chăm sóc tạo hồ sơ sức khỏe cho bệnh nhân"
                };
                _context.LichSuHoSoSucKhoes.Add(lichSu);
            }
            else
            {
                existing.ChieuCao = hoSo.ChieuCao;
                existing.CanNang = hoSo.CanNang;
                existing.NhipTim = hoSo.NhipTim;
                existing.DuongHuyet = hoSo.DuongHuyet;
                existing.HuyetApTamThu = hoSo.HuyetApTamThu;
                existing.HuyetApTamTruong = hoSo.HuyetApTamTruong;
                existing.GhiChu = hoSo.GhiChu;
                existing.NhomMau = hoSo.NhomMau;
                existing.DiUng = hoSo.DiUng;
                existing.TienSuBenh = hoSo.TienSuBenh;
                existing.TienSuGiaDinh = hoSo.TienSuGiaDinh;
                existing.LoiSong = hoSo.LoiSong;
                existing.NgayCapNhat = DateTime.Now;
                
                var lichSu = new LichSuHoSoSucKhoe
                {
                    HoSoSucKhoeId = existing.HoSoSucKhoeId,
                    NguoiThayDoiId = user.Id,
                    NgayThayDoi = DateTime.Now,
                    LoaiThayDoi = "Cập nhật",
                    ThayDoiNoiDung = "Người chăm sóc cập nhật thông tin sức khỏe cho bệnh nhân"
                };
                _context.LichSuHoSoSucKhoes.Add(lichSu);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật hồ sơ sức khỏe thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Caregiver/HealthProfile/History
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string targetUserId = user.Id;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == user.Id);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId!;
            }

            var hoSo = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            if (hoSo == null) return View(new List<LichSuHoSoSucKhoe>());

            var lichSu = await _context.LichSuHoSoSucKhoes
                .Include(l => l.NguoiThayDoi)
                .Where(l => l.HoSoSucKhoeId == hoSo.HoSoSucKhoeId)
                .OrderByDescending(l => l.NgayThayDoi)
                .ToListAsync();

            return View(lichSu);
        }
    }
}
