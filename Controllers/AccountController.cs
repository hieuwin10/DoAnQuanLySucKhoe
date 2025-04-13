using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DoAnChamSocSucKhoe.Data;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.NguoiDungId == int.Parse(user.Id));
            if (nguoiDung == null)
            {
                nguoiDung = new NguoiDung
                {
                    NguoiDungId = int.Parse(user.Id),
                    HoTen = "",
                    Email = "",
                    SoDienThoai = "",
                    MatKhau = "",
                    GioiTinh = "",
                    DiaChi = "",
                    VaiTro = new VaiTro
                    {
                        TenVaiTro = "KhachHang",
                        MoTa = "Khach hang",
                        NguoiDungs = new List<NguoiDung>()
                    },
                    DanhGiaDaGui = new List<DanhGiaChuyenGia>(),
                    ChuyenGias = new List<ChuyenGia>()
                };
                _context.NguoiDungs.Add(nguoiDung);
                await _context.SaveChangesAsync();
            }

            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _context.NguoiDungs.FindAsync(model.NguoiDungId);
                if (nguoiDung == null)
                {
                    return NotFound();
                }

                nguoiDung.HoTen = model.HoTen;
                nguoiDung.NgaySinh = model.NgaySinh;
                nguoiDung.GioiTinh = model.GioiTinh;
                nguoiDung.DiaChi = model.DiaChi;
                nguoiDung.SoDienThoai = model.SoDienThoai;

                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    TempData["Message"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Identity");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
} 