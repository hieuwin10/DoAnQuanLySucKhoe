using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Areas.Caregiver.Controllers
{
    [Area("Caregiver")]
    [Authorize(Roles = "Caregiver")]
    public class TuVanSucKhoeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuVanSucKhoeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Caregiver/TuVanSucKhoe
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            string targetUserId = userId;

            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Where(t => t.NguoiDungId == targetUserId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            ViewBag.DanhSachChuyenGia = await _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .Where(c => c.TrangThai == true)
                .ToListAsync();

            return View(danhSachTuVan);
        }

        // GET: Caregiver/TuVanSucKhoe/Chat/5
        public async Task<IActionResult> Chat(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            string targetUserId = userId;

            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var tuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

            if (tuVan == null)
            {
                return NotFound();
            }

            if (tuVan.NguoiDungId != targetUserId)
            {
                return Forbid();
            }

            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Where(t => t.NguoiDungId == targetUserId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            var hoSoSucKhoe = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            ViewBag.DanhSachTuVan = danhSachTuVan ?? new List<TuVanSucKhoe>();
            ViewBag.HoSoSucKhoe = hoSoSucKhoe;

            return View(tuVan);
        }

        // POST: Caregiver/TuVanSucKhoe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TuVanSucKhoe tuVan)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                string targetUserId = userId;
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

                if (linkedPatient != null && linkedPatient.BenhNhanId != null)
                {
                    targetUserId = linkedPatient.BenhNhanId;
                }

                tuVan.NguoiDungId = targetUserId;
                tuVan.NgayTao = DateTime.Now;
                tuVan.NgayCapNhat = DateTime.Now;
                tuVan.TrangThai = 0;

                _context.Add(tuVan);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Chat), new { id = tuVan.TuVanSucKhoeId });
            }

            ViewBag.DanhSachChuyenGia = await _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .Where(c => c.TrangThai == true)
                .ToListAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Caregiver/TuVanSucKhoe/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int tuVanId, string noiDung)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var tuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == tuVanId);

            if (tuVan == null)
            {
                return NotFound();
            }
            
            bool isAuthorized = tuVan.NguoiDungId == userId;
            
            if (!isAuthorized)
            {
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId && x.BenhNhanId == tuVan.NguoiDungId);
                isAuthorized = linkedPatient != null;
            }
            
            if (!isAuthorized)
            {
                return Forbid();
            }

            var receiverId = tuVan.ChuyenGia.NguoiDungId;

            var message = new DoAnChamSocSucKhoe.Models.Message
            {
                TuVanSucKhoeId = tuVanId,
                SenderId = userId,
                ReceiverId = receiverId,
                Content = noiDung,
                SentTime = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);

            tuVan.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Chat), new { id = tuVanId });
        }

        // POST: Caregiver/TuVanSucKhoe/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var tuVan = await _context.TuVanSucKhoes.FindAsync(id);
            if (tuVan == null)
            {
                return NotFound();
            }

            bool isAuthorized = tuVan.NguoiDungId == userId;
            
            if (!isAuthorized)
            {
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId && x.BenhNhanId == tuVan.NguoiDungId);
                isAuthorized = linkedPatient != null;
            }
            
            if (!isAuthorized)
            {
                return Forbid();
            }

            tuVan.TrangThai = 2; // Đã hủy
            tuVan.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
