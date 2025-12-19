using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System.Security.Claims;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.ChuyenGias)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var chuyenGia = user.ChuyenGias?.FirstOrDefault();
            if (chuyenGia == null)
            {
                // Should potentially redirect to Create Profile if not exists
                 return NotFound("Chưa có thông tin chuyên gia.");
            }

            var model = new DoctorProfileViewModel
            {
                HoTen = user.HoTen,
                Email = user.Email,
                SoDienThoai = user.PhoneNumber,
                ChuyenKhoa = chuyenGia.ChuyenKhoa,
                ChungChi = chuyenGia.ChungChi,
                KinhNghiem = chuyenGia.KinhNghiem,
                NoiCongTac = chuyenGia.NoiCongTac,
                MoTa = chuyenGia.MoTa,
                HinhAnh = chuyenGia.HinhAnh,
                TrangThai = chuyenGia.TrangThai
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(DoctorProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.ChuyenGias)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var chuyenGia = user.ChuyenGias?.FirstOrDefault();
            if (chuyenGia == null)
            {
                // Create new if not exists (though logic implies Doctor role must have one)
                 return NotFound("Chưa có thông tin chuyên gia.");
            }

            // Update User info
            user.HoTen = model.HoTen ?? user.HoTen; // Keep old if null, or use ! since Validated
            user.PhoneNumber = model.SoDienThoai;
            
            // Update ChuyenGia info
            chuyenGia.ChuyenKhoa = model.ChuyenKhoa!;
            chuyenGia.ChungChi = model.ChungChi;
            chuyenGia.KinhNghiem = model.KinhNghiem;
            chuyenGia.NoiCongTac = model.NoiCongTac;
            chuyenGia.MoTa = model.MoTa;
            chuyenGia.NgayCapNhat = DateTime.Now;
            // chuyenGia.TrangThai = model.TrangThai; // Should this be editable? Assuming yes.

            // Handle Image Upload
            if (model.HinhAnhFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "doctors");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.HinhAnhFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhFile.CopyToAsync(fileStream);
                }
                chuyenGia.HinhAnh = "/uploads/doctors/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            
            // Refund model image path to show new image
            model.HinhAnh = chuyenGia.HinhAnh;

            return View(model);
        }
    }
}
