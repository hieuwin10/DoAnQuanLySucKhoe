using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ChatController(
            ApplicationDbContext context,
            UserManager<NguoiDung> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Get consultations for this doctor
            var consultations = await _context.TuVanSucKhoes
                .Where(t => t.ChuyenGiaId == doctorId)
                .Include(t => t.NguoiDung)
                .Include(t => t.Messages.OrderByDescending(m => m.SentTime).Take(1))
                .AsNoTracking()
                .ToListAsync();

            return View(consultations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var consultation = await _context.TuVanSucKhoes
                .Include(t => t.NguoiDung)
                .Include(t => t.Messages.OrderBy(m => m.SentTime))
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id && t.ChuyenGiaId == doctorId);

            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        public async Task<IActionResult> ChatWithPatient(string patientId)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Find existing active consultation
            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.ChuyenGiaId == doctorId && t.NguoiDungId == patientId && t.TrangThai == 1);

            if (consultation == null)
            {
                // Create new consultation if none exists
                // Or maybe find the latest one regardless of status?
                // For now, let's create a new one if no active one found, or redirect to latest if exists.
                
                var latest = await _context.TuVanSucKhoes
                     .Where(t => t.ChuyenGiaId == doctorId && t.NguoiDungId == patientId)
                     .OrderByDescending(t => t.NgayTao)
                     .FirstOrDefaultAsync();

                if (latest != null)
                {
                     return RedirectToAction("Details", new { id = latest.TuVanSucKhoeId });
                }

                // If absolutely no history, maybe create new?
                // For simplicity in this scope, let's create a new "General Consultation".
                var patient = await _context.Users.FindAsync(patientId);
                if (patient == null) return NotFound("Patient not found");

                consultation = new TuVanSucKhoe
                {
                    NguoiDungId = patientId,
                    ChuyenGiaId = doctorId,
                    TieuDe = "Tư vấn sức khỏe mới",
                    NoiDung = "Bắt đầu cuộc trò chuyện mới.",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    TrangThai = 1, // Active
                    NguoiDung = patient,
                    ChuyenGia = await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == doctorId) ?? throw new Exception("Profile not found") 
                };
                
                 _context.TuVanSucKhoes.Add(consultation);
                 await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = consultation.TuVanSucKhoeId });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int consultationId, string content)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == consultationId && t.ChuyenGiaId == doctorId);

            if (consultation == null)
            {
                return Json(new { success = false, message = "Consultation not found" });
            }

            var message = new DoAnChamSocSucKhoe.Models.Message
            {
                TuVanSucKhoeId = consultationId,
                SenderId = doctorId,
                ReceiverId = consultation.NguoiDungId,
                Content = content,
                SentTime = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UploadMedia(int consultationId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "No file uploaded" });
            }

            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == consultationId && t.ChuyenGiaId == doctorId);

            if (consultation == null)
            {
                return Json(new { success = false, message = "Consultation not found" });
            }

            // Validate file type and size
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".avi" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Invalid file type" });
            }

            if (file.Length > 10 * 1024 * 1024) // 10MB limit
            {
                return Json(new { success = false, message = "File too large" });
            }

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var mediaUrl = "/uploads/" + fileName;
            var mediaType = extension == ".mp4" || extension == ".avi" ? "video" : "image";

            // Create message with media
            var message = new DoAnChamSocSucKhoe.Models.Message
            {
                TuVanSucKhoeId = consultationId,
                SenderId = doctorId,
                ReceiverId = consultation.NguoiDungId,
                Content = "", // Media message
                SentTime = DateTime.UtcNow,
                IsRead = false,
                MediaUrl = mediaUrl,
                MediaType = mediaType
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true, mediaUrl, mediaType });
        }

        [HttpPost]
        public async Task<IActionResult> ShareHealthProfile(int consultationId)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var consultation = await _context.TuVanSucKhoes
                .Include(t => t.NguoiDung)
                .ThenInclude(n => n.HoSoSucKhoe)
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == consultationId && t.ChuyenGiaId == doctorId);

            if (consultation == null)
            {
                return Json(new { success = false, message = "Consultation not found" });
            }

            var healthProfile = consultation.NguoiDung.HoSoSucKhoe;
            if (healthProfile == null)
            {
                return Json(new { success = false, message = "No health profile found" });
            }

            var healthInfo = $"Hồ sơ sức khỏe của {consultation.NguoiDung.HoTen}:\n" +
                            $"Chiều cao: {healthProfile.ChieuCao} cm\n" +
                            $"Cân nặng: {healthProfile.CanNang} kg\n" +
                            $"Đường huyết: {healthProfile.DuongHuyet} mg/dL\n" +
                            $"Huyết áp: {healthProfile.HuyetApTamThu}/{healthProfile.HuyetApTamTruong} mmHg\n" +
                            $"Ngày cập nhật: {healthProfile.NgayCapNhat:dd/MM/yyyy}";

            var message = new DoAnChamSocSucKhoe.Models.Message
            {
                TuVanSucKhoeId = consultationId,
                SenderId = doctorId,
                ReceiverId = consultation.NguoiDungId,
                Content = healthInfo,
                SentTime = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}