using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public ReminderController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Patient/Reminder
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "all", string typeFilter = "all", int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            string targetUserId = userId;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var query = _context.NhacNhoSucKhoes
                .Where(r => r.UserId == targetUserId)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.TieuDe.Contains(searchTerm) || r.NoiDung.Contains(searchTerm));
            }

            // Apply status filter
            if (statusFilter == "completed")
            {
                query = query.Where(r => r.DaThucHien);
            }
            else if (statusFilter == "pending")
            {
                query = query.Where(r => !r.DaThucHien);
            }

            // Apply type filter
            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "all")
            {
                query = query.Where(r => r.LoaiNhacNho == typeFilter);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var reminders = await query
                .OrderByDescending(r => r.ThoiGian)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(reminders);
        }

        // GET: Patient/Reminder/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patient/Reminder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhacNhoSucKhoe model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                // Check if user is a caregiver
                var linkedPatient = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
                
                if (linkedPatient != null && linkedPatient.BenhNhanId != null)
                {
                    model.UserId = linkedPatient.BenhNhanId;
                }
                else
                {
                    model.UserId = userId;
                }

                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                model.DaThucHien = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm nhắc nhở thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Patient/Reminder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == userId);

            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // POST: Patient/Reminder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhacNhoSucKhoe model)
        {
            if (id != model.NhacNhoSucKhoeId)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var existingReminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == userId);

            if (existingReminder == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingReminder.TieuDe = model.TieuDe;
                    existingReminder.NoiDung = model.NoiDung;
                    existingReminder.ThoiGian = model.ThoiGian;
                    existingReminder.LoaiNhacNho = model.LoaiNhacNho;
                    existingReminder.NgayCapNhat = DateTime.Now;

                    _context.Update(existingReminder);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật nhắc nhở thành công!";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReminderExists(model.NhacNhoSucKhoeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }

        // GET: Patient/Reminder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == userId);

            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // POST: Patient/Reminder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == userId);

            if (reminder != null)
            {
                _context.NhacNhoSucKhoes.Remove(reminder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa nhắc nhở thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReminderExists(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return _context.NhacNhoSucKhoes.Any(e => e.NhacNhoSucKhoeId == id && e.UserId == userId);
        }

        // GET: Patient/Reminder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            
            string targetUserId = userId;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == targetUserId);

            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == userId);

            if (reminder == null)
            {
                return NotFound();
            }

            if (!reminder.DaThucHien)
            {
                reminder.DaThucHien = true;
                reminder.NgayCapNhat = DateTime.Now;
                _context.Update(reminder);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCompletedAjax(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            
            string targetUserId = userId;
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);
            
            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(m => m.NhacNhoSucKhoeId == id && m.UserId == targetUserId);

            if (reminder == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lời nhắc." });
            }

            if (!reminder.DaThucHien)
            {
                reminder.DaThucHien = true;
                reminder.NgayCapNhat = DateTime.Now;
                _context.Update(reminder);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Cập nhật thành công." });
        }
    }
}
