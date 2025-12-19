using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class NutritionPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NutritionPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Assuming Doctor creates plans, we might want to filter by Doctor's patients or plans created by Doctor.
            // For now, let's list all plans for patients assigned to this doctor, or just all plans if no assignment logic is clear yet.
            // Based on other controllers, there isn't a clear "My Patients" link in the schema visible yet, except via Appointments or Chat.
            // But usually plans are linked to NguoiDung (Patient).
            // Let's just list all for now, or filter if we find a way.
            
            var plans = await _context.KeHoachDinhDuongs
                .Include(p => p.NguoiDung)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();

            return View(plans);
        }

        public IActionResult Create()
        {
            // Need to select a patient
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KeHoachDinhDuong model)
        {
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keHoachDinhDuong = await _context.KeHoachDinhDuongs
                .Include(k => k.NguoiDung)
                .Include(k => k.ChiTietKeHoachDinhDuongs)
                .FirstOrDefaultAsync(m => m.KeHoachDinhDuongId == id);
            if (keHoachDinhDuong == null)
            {
                return NotFound();
            }

            return View(keHoachDinhDuong);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keHoachDinhDuong = await _context.KeHoachDinhDuongs.FindAsync(id);
            if (keHoachDinhDuong == null)
            {
                return NotFound();
            }
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View(keHoachDinhDuong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KeHoachDinhDuong keHoachDinhDuong)
        {
            if (id != keHoachDinhDuong.KeHoachDinhDuongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    keHoachDinhDuong.NgayCapNhat = DateTime.Now;
                    _context.Update(keHoachDinhDuong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeHoachDinhDuongExists(keHoachDinhDuong.KeHoachDinhDuongId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View(keHoachDinhDuong);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keHoachDinhDuong = await _context.KeHoachDinhDuongs
                .Include(k => k.NguoiDung)
                .FirstOrDefaultAsync(m => m.KeHoachDinhDuongId == id);
            if (keHoachDinhDuong == null)
            {
                return NotFound();
            }

            return View(keHoachDinhDuong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keHoachDinhDuong = await _context.KeHoachDinhDuongs.FindAsync(id);
            if (keHoachDinhDuong != null)
            {
                _context.KeHoachDinhDuongs.Remove(keHoachDinhDuong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeHoachDinhDuongExists(int id)
        {
            return _context.KeHoachDinhDuongs.Any(e => e.KeHoachDinhDuongId == id);
        }
    }
}
