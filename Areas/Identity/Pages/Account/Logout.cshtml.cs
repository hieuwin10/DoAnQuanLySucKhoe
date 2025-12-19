using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DoAnChamSocSucKhoe.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<NguoiDung> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
          public IActionResult OnGet()
        {
            // Nếu người dùng truy cập trực tiếp vào trang Logout.cshtml
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation($"Người dùng đã xác thực đang truy cập trang đăng xuất trực tiếp");
            }
            
            return Page();
        }
        
        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            var userName = User.Identity?.Name;
            _logger.LogInformation($"Đang xử lý đăng xuất cho người dùng: {userName}");
            
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"Người dùng {userName} đã đăng xuất thành công");
            
            if (returnUrl != null)
            {
                _logger.LogInformation($"Chuyển hướng đến: {returnUrl}");
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                _logger.LogInformation("Chuyển hướng đến trang chủ");
                return RedirectToPage("/Index", new { area = "" });
            }
        }
    }
}
