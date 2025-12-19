using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DoAnChamSocSucKhoe.Data;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<NguoiDung> userManager,
            SignInManager<NguoiDung> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.Id == user.Id);
            if (nguoiDung == null)
            {
                nguoiDung = new NguoiDung
                {
                    Id = user.Id,
                    HoTen = "",
                    Email = "",
                    PhoneNumber = "",
                    GioiTinh = "",
                    DiaChi = "",
                    VaiTro = GetVaiTro("KhachHang") ?? new VaiTro
                    {
                        TenVaiTro = "KhachHang",
                        MoTa = "Khách hàng",
                        NguoiDungs = new List<NguoiDung>()
                    },
                    DanhGiaDaGui = new List<DanhGiaChuyenGia>(),
                    ChuyenGias = new List<ChuyenGia>()
                };
                _context.NguoiDungs.Add(nguoiDung);
                await _context.SaveChangesAsync();
            }

            if (nguoiDung == null)
            {
                return NotFound(); // Or redirect to a "create profile" page
            }

            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _context.NguoiDungs.FindAsync(model.Id);
                if (nguoiDung == null)
                {
                    return NotFound();
                }

                nguoiDung.HoTen = model.HoTen;
                nguoiDung.NgaySinh = model.NgaySinh ?? nguoiDung.NgaySinh;
                nguoiDung.GioiTinh = model.GioiTinh ?? nguoiDung.GioiTinh;
                nguoiDung.DiaChi = model.DiaChi ?? nguoiDung.DiaChi;
                nguoiDung.PhoneNumber = model.PhoneNumber ?? nguoiDung.PhoneNumber;

                _context.Update(nguoiDung);                // Log the data modification
                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO DataModificationLogs (Timestamp, User, TableName, PrimaryKey, OriginalValue, NewValue) " +
                    "VALUES (GETDATE(), {0}, {1}, {2}, {3}, {4})",
                    User.Identity != null ? User.Identity.Name : "",
                    "NguoiDung",
                    nguoiDung.Id,
                    nguoiDung.PhoneNumber,
                    model.PhoneNumber);

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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (roles.Contains("Doctor"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
                    }
                    else if (roles.Contains("Patient"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Patient" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "HealthProfile", new { area = "" });
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", "Identity", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout", "Identity", new { area = "Identity" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new NguoiDung
                {
                    UserName = model.Email,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    VaiTroId = GetVaiTroId(model.VaiTro)
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, model.VaiTro);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // TODO: Send email confirmation

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl ?? "/");
                }
                AddErrors(result);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Landing", "Home", new { area = "" });
            }
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
        {
            return View();
        }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { code = code },
                    protocol: Request.Scheme);

                // TODO: Send email with reset password link

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code, Password = "", ConfirmPassword = "", Email = "" };
            return View(model);
        }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code ?? string.Empty, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

        private VaiTro GetVaiTro(string tenVaiTro)
        {
            return _context.VaiTros.FirstOrDefault(v => v.TenVaiTro == tenVaiTro) ?? new VaiTro
            {
                TenVaiTro = "KhachHang",
                MoTa = "Khách hàng",
                NguoiDungs = new List<NguoiDung>()
            };
        }

        private int GetVaiTroId(string tenVaiTro)
        {
            var vaiTro = _context.VaiTros.FirstOrDefault(v => v.TenVaiTro == tenVaiTro);
            return vaiTro?.VaiTroId ?? 0;
        }
    }
}