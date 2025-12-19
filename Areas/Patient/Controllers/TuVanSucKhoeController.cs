using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Policy = "CanSendConsultation")]
    public class TuVanSucKhoeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuVanSucKhoeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patient/TuVanSucKhoe
        public async Task<IActionResult> Index()
        {
            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            string targetUserId = userId;

            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            // Lấy danh sách tư vấn của người dùng hiện tại (hoặc bệnh nhân được chăm sóc)
            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Where(t => t.NguoiDungId == targetUserId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            // Lấy danh sách chuyên gia đang hoạt động để hiển thị modal tạo tư vấn mới
            ViewBag.DanhSachChuyenGia = await _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .Where(c => c.TrangThai == true)
                .ToListAsync();

            return View(danhSachTuVan);
        }

        // GET: Patient/TuVanSucKhoe/Chat/5
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

            // Lấy danh sách tư vấn khác để hiển thị trong sidebar
            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Where(t => t.NguoiDungId == targetUserId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            // Lấy hồ sơ sức khỏe để hiển thị thông tin bên sidebar
            var hoSoSucKhoe = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == targetUserId);

            // Fix possible null reference assignment
            ViewBag.DanhSachTuVan = danhSachTuVan ?? new List<TuVanSucKhoe>();
            ViewBag.HoSoSucKhoe = hoSoSucKhoe; // This is fine as ViewBag can accept null values

            return View(tuVan);
        }

        // POST: Patient/TuVanSucKhoe/Create
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
                tuVan.TrangThai = 0; // 0 = Chờ tư vấn

                _context.Add(tuVan);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Chat), new { id = tuVan.TuVanSucKhoeId });
            }

            ViewBag.DanhSachChuyenGia = await _context.ChuyenGias.ToListAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Patient/TuVanSucKhoe/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int tuVanId, string noiDung)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var tuVan = await _context.TuVanSucKhoes.FindAsync(tuVanId);

            if (tuVan == null)
            {
                return NotFound();
            }
            
            // Check if the user is the owner or a linked caregiver
            bool isAuthorized = tuVan.NguoiDungId == userId;
            if (!isAuthorized)
            {
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId && x.BenhNhanId == tuVan.NguoiDungId);
                if (linkedPatient != null) isAuthorized = true;
            }

            if (!isAuthorized)
            {
                return Forbid();
            }

            // Trong thực tế bạn sẽ lưu tin nhắn này vào bảng tin nhắn
            // Nhưng hiện tại chúng ta sẽ cập nhật nội dung tư vấn
            tuVan.NoiDung += $"\n[Bệnh nhân] {DateTime.Now}: {noiDung}";
            tuVan.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Chat), new { id = tuVanId });
        }

        // POST: Patient/TuVanSucKhoe/Cancel
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

            // Check if the user is the owner or a linked caregiver
            bool isAuthorized = tuVan.NguoiDungId == userId;
            if (!isAuthorized)
            {
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId && x.BenhNhanId == tuVan.NguoiDungId);
                if (linkedPatient != null) isAuthorized = true;
            }

            if (!isAuthorized)
            {
                return Forbid();
            }

            tuVan.TrangThai = 2; // 2 = Đã hủy
            tuVan.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}