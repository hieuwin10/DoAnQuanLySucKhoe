using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HealthProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public HealthProfileController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/HealthProfile - Dashboard
        public async Task<IActionResult> Index()
        {
            var totalProfiles = await _context.HoSoSucKhoes.CountAsync();
            var completeProfiles = await _context.HoSoSucKhoes
                .Where(h => h.TrangThai == "Hoàn chỉnh" || h.TrangThai == "Tốt")
                .CountAsync();
            var todayUpdates = await _context.HoSoSucKhoes
                .Where(h => h.NgayCapNhat.Date == DateTime.Today)
                .CountAsync();
            var alertsCount = await _context.HoSoSucKhoes
                .Where(h => h.TrangThai == "Cần chú ý" || h.TrangThai == "Nguy hiểm")
                .CountAsync();

            ViewBag.TotalProfiles = totalProfiles;
            ViewBag.CompleteProfiles = completeProfiles;
            ViewBag.TodayUpdates = todayUpdates;
            ViewBag.AlertsCount = alertsCount;

            return View();
        }

        // GET: Admin/HealthProfile/ViewAll
        public async Task<IActionResult> ViewAll(string searchString, string filterStatus, int page = 1)
        {
            int pageSize = 20;
            var query = _context.HoSoSucKhoes
                .Include(h => h.NguoiDung)
                .ThenInclude(u => u.VaiTro)
                .Where(h => h.NguoiDung.VaiTro.TenVaiTro == "Patient")
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(h => 
                    h.NguoiDung != null && (h.NguoiDung.HoTen.Contains(searchString) || 
                    h.NguoiDung.Email.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(filterStatus))
            {
                query = query.Where(h => h.TrangThai == filterStatus);
            }

            var totalItems = await query.CountAsync();
            var profiles = await query
                .OrderByDescending(h => h.NgayCapNhat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.FilterStatus = filterStatus;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(profiles);
        }

        // GET: Admin/HealthProfile/Statistics
        public async Task<IActionResult> Statistics(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Now.AddMonths(-6);
            endDate ??= DateTime.Now;

            // Monthly profile updates
            var monthlyUpdates = await _context.LichSuHoSoSucKhoes
                .Where(l => l.NgayThayDoi >= startDate && l.NgayThayDoi <= endDate)
                .GroupBy(l => new { l.NgayThayDoi.Year, l.NgayThayDoi.Month })
                .Select(g => new { 
                    Year = g.Key.Year, 
                    Month = g.Key.Month, 
                    Count = g.Count() 
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            ViewBag.MonthlyUpdates = monthlyUpdates;

            // Update type distribution
            var updateTypes = await _context.LichSuHoSoSucKhoes
                .Where(l => l.NgayThayDoi >= startDate && l.NgayThayDoi <= endDate)
                .GroupBy(l => l.LoaiThayDoi)
                .Select(g => new { Type = g.Key ?? "Khác", Count = g.Count() })
                .ToListAsync();

            ViewBag.UpdateTypes = updateTypes;

            // File uploads statistics
            var fileStats = await _context.FileHoSos
                .Where(f => f.NgayTaiLen >= startDate && f.NgayTaiLen <= endDate)
                .GroupBy(f => f.LoaiFile)
                .Select(g => new { Type = g.Key ?? "Khác", Count = g.Count() })
                .ToListAsync();

            ViewBag.FileStats = fileStats;

            // Health metrics averages
            var avgMetrics = await _context.HoSoSucKhoes
                .Where(h => h.NgayCapNhat >= startDate && h.NgayCapNhat <= endDate)
                .GroupBy(h => 1)
                .Select(g => new {
                    AvgHeight = g.Average(h => h.ChieuCao),
                    AvgWeight = g.Average(h => h.CanNang),
                    AvgHeartRate = g.Average(h => h.NhipTim),
                    AvgBloodSugar = g.Average(h => h.DuongHuyet),
                    AvgSystolic = g.Average(h => h.HuyetApTamThu),
                    AvgDiastolic = g.Average(h => h.HuyetApTamTruong)
                })
                .FirstOrDefaultAsync();

            ViewBag.AvgMetrics = avgMetrics;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View();
        }

        // GET: Admin/HealthProfile/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hoSo = await _context.HoSoSucKhoes
                .Include(h => h.NguoiDung)
                .FirstOrDefaultAsync(h => h.HoSoSucKhoeId == id);

            if (hoSo == null) return NotFound();

            // Get full history
            ViewBag.LichSu = await _context.LichSuHoSoSucKhoes
                .Include(l => l.NguoiThayDoi)
                .Where(l => l.HoSoSucKhoeId == id)
                .OrderByDescending(l => l.NgayThayDoi)
                .ToListAsync();

            // Get all files
            ViewBag.Files = await _context.FileHoSos
                .Include(f => f.NguoiTaiLen)
                .Where(f => f.HoSoSucKhoeId == id)
                .OrderByDescending(f => f.NgayTaiLen)
                .ToListAsync();

            return View(hoSo);
        }

        // GET: Admin/HealthProfile/ManageAccess
        public async Task<IActionResult> ManageAccess()
        {
            var users = await _context.NguoiDungs
                .Include(n => n.VaiTro)
                .Include(n => n.HoSoSucKhoe)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            return View(users);
        }

        // POST: Admin/HealthProfile/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hoSo = await _context.HoSoSucKhoes.FindAsync(id);
            if (hoSo == null) return NotFound();

            // Delete related records first
            var lichSu = _context.LichSuHoSoSucKhoes.Where(l => l.HoSoSucKhoeId == id);
            _context.LichSuHoSoSucKhoes.RemoveRange(lichSu);

            var files = _context.FileHoSos.Where(f => f.HoSoSucKhoeId == id);
            _context.FileHoSos.RemoveRange(files);

            _context.HoSoSucKhoes.Remove(hoSo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa hồ sơ sức khỏe thành công!";
            return RedirectToAction(nameof(ViewAll));
        }

        // GET: Admin/HealthProfile/Export
        public async Task<IActionResult> Export(DateTime? startDate, DateTime? endDate, string format = "csv")
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var profiles = await _context.HoSoSucKhoes
                .Include(h => h.NguoiDung)
                .Where(h => h.NgayCapNhat >= startDate && h.NgayCapNhat <= endDate)
                .OrderByDescending(h => h.NgayCapNhat)
                .ToListAsync();

            if (format == "csv")
            {
                var csv = "ID,Họ tên,Email,Chiều cao,Cân nặng,Nhịp tim,Đường huyết,Huyết áp tâm thu,Huyết áp tâm trương,Trạng thái,Ngày cập nhật\n";
                foreach (var p in profiles)
                {
                    csv += $"{p.HoSoSucKhoeId},{p.NguoiDung?.HoTen},{p.NguoiDung?.Email},{p.ChieuCao},{p.CanNang},{p.NhipTim},{p.DuongHuyet},{p.HuyetApTamThu},{p.HuyetApTamTruong},{p.TrangThai},{p.NgayCapNhat:yyyy-MM-dd}\n";
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"health_profiles_{DateTime.Now:yyyyMMdd}.csv");
            }

            return RedirectToAction(nameof(ViewAll));
        }
    }
}
