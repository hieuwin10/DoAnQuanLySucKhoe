using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize]
    public class TuVanSucKhoeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuVanSucKhoeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TuVanSucKhoe
        public async Task<IActionResult> Index()
        {
            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;

            // Lấy danh sách tư vấn của người dùng hiện tại
            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Where(t => t.NguoiDungId == userId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            // Lấy danh sách chuyên gia để hiển thị modal tạo tư vấn mới
            ViewBag.DanhSachChuyenGia = await _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .ToListAsync();

            return View(danhSachTuVan);
        }

        // GET: TuVanSucKhoe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .ThenInclude(c => c.NguoiDung)
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(m => m.TuVanSucKhoeId == id);

            if (tuVan == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có quyền xem tư vấn này không
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;

            if (tuVan.NguoiDungId != userId && !User.IsInRole("Admin") && !User.IsInRole("ChuyenGia"))
            {
                return Forbid();
            }

            return View(tuVan);
        }

        // GET: TuVanSucKhoe/Create
        public IActionResult Create()
        {
            ViewBag.DanhSachChuyenGia = _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .ToList();
            return View();
        }

        // POST: TuVanSucKhoe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TieuDe,NoiDung,ChuyenGiaId")] TuVanSucKhoe tuVan)
        {
            if (ModelState.IsValid)
            {
                // Thiết lập thông tin bổ sung
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "Unable to determine the current user.");
                    ViewBag.DanhSachChuyenGia = _context.ChuyenGias
                        .Include(c => c.NguoiDung)
                        .ToList();
                    return View(tuVan);
                }
                tuVan.NguoiDungId = userId;
                tuVan.NgayTao = DateTime.Now;
                tuVan.NgayCapNhat = DateTime.Now;
                tuVan.TrangThai = 0; // 0 = Chờ tư vấn
                tuVan.TraLoi = ""; // Khởi tạo trả lời rỗng

                _context.Add(tuVan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = tuVan.TuVanSucKhoeId });
            }

            ViewBag.DanhSachChuyenGia = _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .ToList();
            return View(tuVan);
        }

        // GET: TuVanSucKhoe/Chat/5
        public async Task<IActionResult> Chat(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                    .ThenInclude(c => c.NguoiDung)
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(m => m.TuVanSucKhoeId == id);

            if (tuVan == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có quyền xem tư vấn này không
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
            var isChuyenGia = User.IsInRole("ChuyenGia");

            if (tuVan.NguoiDungId != userId && !User.IsInRole("Admin") && !isChuyenGia)
            {
                return Forbid();
            }

            // Lấy thêm danh sách tư vấn của người dùng hiện tại để hiển thị sidebar
            var danhSachTuVan = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                    .ThenInclude(c => c.NguoiDung)
                .Include(t => t.NguoiDung)
                .Where(t => t.NguoiDungId == userId)
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();

            // Fix possible null reference assignment
            ViewBag.DanhSachTuVan = danhSachTuVan ?? new List<TuVanSucKhoe>();
            ViewBag.IsChuyenGia = isChuyenGia;

            return View(tuVan);
        }

        // POST: TuVanSucKhoe/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string traLoi)
        {
            var tuVan = await _context.TuVanSucKhoes.FindAsync(id);

            if (tuVan == null)
            {
                return NotFound();
            }

            // Cập nhật câu trả lời và trạng thái
            tuVan.TraLoi = traLoi;
            tuVan.TrangThai = 1; // 1 = Đã tư vấn
            tuVan.NgayCapNhat = DateTime.Now;

            _context.Update(tuVan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Chat), new { id = tuVan.TuVanSucKhoeId });
        }

        // POST: TuVanSucKhoe/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var tuVan = await _context.TuVanSucKhoes.FindAsync(id);

            if (tuVan == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái
            tuVan.TrangThai = 2; // 2 = Huỷ
            tuVan.NgayCapNhat = DateTime.Now;

            _context.Update(tuVan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}