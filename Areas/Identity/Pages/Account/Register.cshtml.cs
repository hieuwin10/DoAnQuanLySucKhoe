using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IUserStore<NguoiDung> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<NguoiDung> userManager,
            IUserStore<NguoiDung> userStore,
            SignInManager<NguoiDung> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; } = string.Empty;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            [Display(Name = "Họ tên")]
            public string HoTen { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng chọn vai trò")]
            [Display(Name = "Vai trò")]
            public string VaiTro { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == Input.VaiTro);
            if (vaiTro == null)
            {
                ModelState.AddModelError(string.Empty, "Vai trò không hợp lệ.");
                return Page();
            }

            var user = new NguoiDung
            {
                UserName = Input.Email,
                Email = Input.Email,
                HoTen = Input.HoTen,
                PhoneNumber = "", // Default to empty, can be updated later
                VaiTroId = vaiTro.VaiTroId,
                TrangThai = "Hoạt động",
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now,
                EmailConfirmed = true // Automatically confirm email
            };
            
            var result = await _userManager.CreateAsync(user, Input.Password);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            _logger.LogInformation("User created a new account with password.");

            var roleResult = await _userManager.AddToRoleAsync(user, Input.VaiTro);
            if (!roleResult.Succeeded)
            {
                _logger.LogError($"Could not add user {user.Email} to role {Input.VaiTro}");
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await _userManager.DeleteAsync(user); // Delete user if role assignment fails
                return Page();
            }

            _logger.LogInformation($"Added user {user.Email} to role {Input.VaiTro}");

            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect based on role
            return Input.VaiTro switch
            {
                "Admin" => LocalRedirect("~/Admin/Dashboard"),
                "Doctor" => LocalRedirect("~/Doctor/Dashboard"),
                "Patient" => LocalRedirect("~/Patient/Dashboard"),
                _ => LocalRedirect(returnUrl)
            };
        }
    }
}