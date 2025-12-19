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
    public class DanhGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctor/DanhGia
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chuyenGia = await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == userId);

            if (chuyenGia == null)
            {
                return NotFound("Không tìm thấy thông tin chuyên gia.");
            }

            var reviews = await _context.DanhGiaChuyenGias
                .Include(d => d.NguoiDung)
                .Include(d => d.TuVanSucKhoe)
                .Where(d => d.ChuyenGiaId == chuyenGia.ChuyenGiaId)
                .OrderByDescending(d => d.Id) // Assuming Id is auto-increment, otherwise use created date if available
                .ToListAsync();

            return View(reviews);
        }

        // GET: Doctor/DanhGia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chuyenGia = await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == userId);

            if (chuyenGia == null)
            {
                return NotFound("Không tìm thấy thông tin chuyên gia.");
            }

            var danhGia = await _context.DanhGiaChuyenGias
                .Include(d => d.NguoiDung)
                .Include(d => d.TuVanSucKhoe)
                .FirstOrDefaultAsync(m => m.Id == id && m.ChuyenGiaId == chuyenGia.ChuyenGiaId);

            if (danhGia == null)
            {
                return NotFound();
            }

            return View(danhGia);
        }
    }
}
