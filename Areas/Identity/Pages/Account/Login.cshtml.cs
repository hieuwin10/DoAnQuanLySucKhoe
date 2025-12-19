using System.ComponentModel.DataAnnotations;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoAnChamSocSucKhoe.Data;
using Microsoft.AspNetCore.Authentication;

namespace DoAnChamSocSucKhoe.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<NguoiDung> _userManager;

        public LoginModel(SignInManager<NguoiDung> signInManager, ILogger<LoginModel> logger, UserManager<NguoiDung> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Ghi nhớ đăng nhập")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = "")
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl ?? Url.Content("~/");
            ViewData["ReturnUrl"] = ReturnUrl;
            }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "")
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {                // Ghi log chi tiết cho debugging
                _logger.LogInformation($"Đang thử đăng nhập với email: {Input.Email}");
                
                // Kiểm tra xem người dùng có tồn tại trong AspNetUsers
                var userExists = await _userManager.FindByEmailAsync(Input.Email);
                if (userExists == null)
                {
                    _logger.LogWarning($"User không tồn tại trong AspNetUsers với email: {Input.Email}");
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                    return Page();
                }
                
                _logger.LogInformation($"Tìm thấy user trong AspNetUsers với ID: {userExists.Id}");
                
                // Kiểm tra trạng thái EmailConfirmed
                _logger.LogInformation($"User {Input.Email} có EmailConfirmed = {userExists.EmailConfirmed}");
                
                // Thực hiện đăng nhập                // Thử đăng nhập bằng tên người dùng (email) đầu tiên
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                // Nếu đăng nhập bằng email không thành công, thử sử dụng email làm UserName
                if (!result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null && !string.IsNullOrEmpty(user.UserName))
                    {
                        _logger.LogInformation($"Thử đăng nhập bằng UserName: {user.UserName}");
                        result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    }
                }
                
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {Input.Email} đăng nhập thành công.");
                    
                    // Lấy vai trò của người dùng
                    var roles = await _userManager.GetRolesAsync(userExists);
                    _logger.LogInformation($"Vai trò của user {Input.Email}: {string.Join(", ", roles)}");
                          // Không cần kiểm tra NguoiDung trực tiếp ở đây nữa
                    
                    // Chuyển hướng theo vai trò
                    if (roles.Contains("Admin"))
                    {
                        return LocalRedirect("~/Admin/Dashboard");
                    }
                    else if (roles.Contains("Doctor"))
                    {
                        return LocalRedirect("~/Doctor/Dashboard");
                    }
                    else if (roles.Contains("Patient"))
                    {
                        return LocalRedirect("~/Patient/Dashboard");
                    }
                    
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    _logger.LogWarning($"Đăng nhập thất bại cho email {Input.Email}. Lý do: {result}");
                
                if (userExists?.PasswordHash != null)
                {
                    _logger.LogWarning($"PasswordHash cho {Input.Email}: {userExists.PasswordHash.Substring(0, Math.Min(10, userExists.PasswordHash.Length))}...");
                }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");
                    return Page();
                }
            }

            return Page();
        }
    }
} 