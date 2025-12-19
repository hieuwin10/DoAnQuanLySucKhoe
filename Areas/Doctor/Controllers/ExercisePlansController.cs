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
    public class ExercisePlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisePlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _context.KeHoachTapLuyens
                .Include(p => p.NguoiDung)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();

            return View(plans);
        }

        public IActionResult Create()
        {
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KeHoachTapLuyen model)
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

            var keHoachTapLuyen = await _context.KeHoachTapLuyens
                .Include(k => k.NguoiDung)
                .Include(k => k.ChiTietKeHoachTapLuyens)
                .FirstOrDefaultAsync(m => m.KeHoachTapLuyenId == id);
            if (keHoachTapLuyen == null)
            {
                return NotFound();
            }

            return View(keHoachTapLuyen);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keHoachTapLuyen = await _context.KeHoachTapLuyens.FindAsync(id);
            if (keHoachTapLuyen == null)
            {
                return NotFound();
            }
            ViewBag.Patients = _context.Users.Where(u => u.VaiTro.TenVaiTro == "Patient").ToList();
            return View(keHoachTapLuyen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KeHoachTapLuyen keHoachTapLuyen)
        {
            if (id != keHoachTapLuyen.KeHoachTapLuyenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    keHoachTapLuyen.NgayCapNhat = DateTime.Now;
                    _context.Update(keHoachTapLuyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeHoachTapLuyenExists(keHoachTapLuyen.KeHoachTapLuyenId))
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
            return View(keHoachTapLuyen);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keHoachTapLuyen = await _context.KeHoachTapLuyens
                .Include(k => k.NguoiDung)
                .FirstOrDefaultAsync(m => m.KeHoachTapLuyenId == id);
            if (keHoachTapLuyen == null)
            {
                return NotFound();
            }

            return View(keHoachTapLuyen);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keHoachTapLuyen = await _context.KeHoachTapLuyens.FindAsync(id);
            if (keHoachTapLuyen != null)
            {
                _context.KeHoachTapLuyens.Remove(keHoachTapLuyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeHoachTapLuyenExists(int id)
        {
            return _context.KeHoachTapLuyens.Any(e => e.KeHoachTapLuyenId == id);
        }
    }
}
