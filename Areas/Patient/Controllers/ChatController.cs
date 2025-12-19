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

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
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
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Get consultations for this patient
            var consultations = await _context.TuVanSucKhoes
                .Where(t => t.NguoiDungId == patientId)
                .Include(t => t.ChuyenGia)
                .Include(t => t.Messages.OrderByDescending(m => m.SentTime).Take(1)) // Lazy load last message
                .AsNoTracking()
                .ToListAsync();

            return View(consultations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var consultation = await _context.TuVanSucKhoes
                .Include(t => t.ChuyenGia)
                .Include(t => t.Messages.OrderBy(m => m.SentTime))
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id && t.NguoiDungId == patientId);

            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int consultationId, string content)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == consultationId && t.NguoiDungId == patientId);

            if (consultation == null)
            {
                return Json(new { success = false, message = "Consultation not found" });
            }

            var message = new DoAnChamSocSucKhoe.Models.Message
            {
                TuVanSucKhoeId = consultationId,
                SenderId = patientId,
                ReceiverId = consultation.ChuyenGiaId,
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

            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == consultationId && t.NguoiDungId == patientId);

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
                SenderId = patientId,
                ReceiverId = consultation.ChuyenGiaId,
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
    }
}