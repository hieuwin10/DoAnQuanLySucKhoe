using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class HealthProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public HealthProfileController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Doctor/HealthProfile - Danh sách bệnh nhân
        public async Task<IActionResult> Index(string searchString, string filterStatus)
        {
            var patientsQuery = _context.NguoiDungs
                .Include(n => n.HoSoSucKhoe)
                .Include(n => n.VaiTro)
                .Where(n => n.VaiTro != null && n.VaiTro.TenVaiTro == "Patient");

            if (!string.IsNullOrEmpty(searchString))
            {
                patientsQuery = patientsQuery.Where(p => 
                    p.HoTen.Contains(searchString) || 
                    p.Email.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(filterStatus))
            {
                patientsQuery = patientsQuery.Where(p => 
                    p.HoSoSucKhoe != null && p.HoSoSucKhoe.TrangThai == filterStatus);
            }

            var patients = await patientsQuery
                .OrderByDescending(p => p.NgayCapNhat)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.FilterStatus = filterStatus;
            
            return View(patients);
        }

        // GET: Doctor/HealthProfile/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var patient = await _context.NguoiDungs
                .Include(n => n.HoSoSucKhoe)
                .Include(n => n.ChiSoSucKhoes)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (patient == null) return NotFound();

            // Get health history
            if (patient.HoSoSucKhoe != null)
            {
                ViewBag.LichSu = await _context.LichSuHoSoSucKhoes
                    .Include(l => l.NguoiThayDoi)
                    .Where(l => l.HoSoSucKhoeId == patient.HoSoSucKhoe.HoSoSucKhoeId)
                    .OrderByDescending(l => l.NgayThayDoi)
                    .Take(10)
                    .ToListAsync();

                // Get uploaded files
                ViewBag.Files = await _context.FileHoSos
                    .Include(f => f.NguoiTaiLen)
                    .Where(f => f.HoSoSucKhoeId == patient.HoSoSucKhoe.HoSoSucKhoeId)
                    .OrderByDescending(f => f.NgayTaiLen)
                    .ToListAsync();
            }
            else
            {
                ViewBag.LichSu = new List<LichSuHoSoSucKhoe>();
                ViewBag.Files = new List<FileHoSo>();
            }

            return View(patient.HoSoSucKhoe);
        }

        // GET: Doctor/HealthProfile/Update/5
        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var patient = await _context.NguoiDungs
                .Include(n => n.HoSoSucKhoe)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (patient == null) return NotFound();

            if (patient.HoSoSucKhoe == null)
            {
                patient.HoSoSucKhoe = new HoSoSucKhoe
                {
                    NguoiDungId = patient.Id,
                    NgayCapNhat = DateTime.Now
                };
            }

            return View(patient.HoSoSucKhoe);
        }

        // POST: Doctor/HealthProfile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, HoSoSucKhoe hoSo)
        {
            if (hoSo == null) return BadRequest("Invalid data.");

            // id from URL is UserId (string), but we are updating HoSoSucKhoe
            // We can verify if the profile belongs to the user if needed, 
            // or just rely on HoSoSucKhoeId from the form.
            
            var existing = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.HoSoSucKhoeId == hoSo.HoSoSucKhoeId);

            if (existing == null) return NotFound();

            // Optional: Verify User ID matches
            if (existing.NguoiDungId != id) 
            {
                 // If the URL ID doesn't match the profile's User ID, it might be a mismatch
                 // But strictly speaking, we are updating 'existing' found by HoSoSucKhoeId
            }

            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null) return Unauthorized();

            // Update medical information (doctor-specific fields)
            existing.ChanDoan = hoSo.ChanDoan;
            existing.PhuongPhapDieuTri = hoSo.PhuongPhapDieuTri;
            existing.TrangThai = hoSo.TrangThai;
            existing.TienSuBenh = hoSo.TienSuBenh;
            existing.TienSuGiaDinh = hoSo.TienSuGiaDinh;
            existing.DiUng = hoSo.DiUng;
            existing.ThuocDangDung = hoSo.ThuocDangDung;
            existing.GhiChu = hoSo.GhiChu;
            existing.NgayCapNhat = DateTime.Now;

            // Log the change
            var lichSu = new LichSuHoSoSucKhoe
            {
                HoSoSucKhoeId = existing.HoSoSucKhoeId,
                NguoiThayDoiId = doctor?.Id,
                NgayThayDoi = DateTime.Now,
                LoaiThayDoi = "Bác sĩ cập nhật",
                ThayDoiNoiDung = $"Bác sĩ {doctor?.HoTen} cập nhật chẩn đoán và phương pháp điều trị"
            };
            _context.LichSuHoSoSucKhoes.Add(lichSu);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật hồ sơ bệnh nhân thành công!";
            
            return RedirectToAction(nameof(Details), new { id = existing.NguoiDungId });
        }

        // POST: Doctor/HealthProfile/AddNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int hoSoId, string note)
        {
            if (string.IsNullOrWhiteSpace(note)) return BadRequest("Note cannot be empty.");

            var hoSo = await _context.HoSoSucKhoes.FindAsync(hoSoId);
            if (hoSo == null) return NotFound();

            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null) return Unauthorized();

            var lichSu = new LichSuHoSoSucKhoe
            {
                HoSoSucKhoeId = hoSoId,
                NguoiThayDoiId = doctor?.Id,
                NgayThayDoi = DateTime.Now,
                LoaiThayDoi = "Ghi chú y khoa",
                ThayDoiNoiDung = note
            };
            _context.LichSuHoSoSucKhoes.Add(lichSu);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm ghi chú thành công!";

            return RedirectToAction(nameof(Details), new { id = hoSo.NguoiDungId });
        }

        // GET: Doctor/HealthProfile/Trend/5
        public async Task<IActionResult> Trend(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var patient = await _context.NguoiDungs
                .Include(n => n.ChiSoSucKhoes)
                .Include(n => n.HoSoSucKhoe)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (patient == null) return NotFound();

            var healthMetrics = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == id)
                .OrderBy(c => c.NgayDo)
                .ToListAsync();

            ViewBag.PatientName = patient.HoTen;
            return View(healthMetrics);
        }
    }
}
