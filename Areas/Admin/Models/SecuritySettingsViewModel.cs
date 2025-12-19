using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class SecuritySettingsViewModel
    {
        [Display(Name = "Bật xác thực hai yếu tố")]
        public bool EnableTwoFactorAuth { get; set; } = false;

        [Display(Name = "Độ dài mật khẩu tối thiểu")]
        [Range(6, 128, ErrorMessage = "Độ dài mật khẩu phải từ 6 đến 128 ký tự")]
        public int PasswordMinLength { get; set; } = 8;

        [Display(Name = "Yêu cầu chữ hoa")]
        public bool PasswordRequireUppercase { get; set; } = true;

        [Display(Name = "Yêu cầu chữ thường")]
        public bool PasswordRequireLowercase { get; set; } = true;

        [Display(Name = "Yêu cầu chữ số")]
        public bool PasswordRequireDigit { get; set; } = true;

        [Display(Name = "Yêu cầu ký tự đặc biệt")]
        public bool PasswordRequireNonAlphanumeric { get; set; } = false;

        [Display(Name = "Số lần đăng nhập tối đa")]
        [Range(3, 10, ErrorMessage = "Số lần đăng nhập tối đa phải từ 3 đến 10")]
        public int MaxLoginAttempts { get; set; } = 5;

        [Display(Name = "Thời gian khóa tài khoản (phút)")]
        [Range(5, 1440, ErrorMessage = "Thời gian khóa phải từ 5 phút đến 24 giờ")]
        public int LockoutDurationMinutes { get; set; } = 15;
    }
}