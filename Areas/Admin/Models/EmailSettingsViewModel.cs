using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class EmailSettingsViewModel
    {
        [Display(Name = "Máy chủ SMTP")]
        [Required(ErrorMessage = "Máy chủ SMTP là bắt buộc")]
        [StringLength(100, ErrorMessage = "Máy chủ SMTP không được vượt quá 100 ký tự")]
        public string SmtpServer { get; set; } = "";

        [Display(Name = "Cổng SMTP")]
        [Required(ErrorMessage = "Cổng SMTP là bắt buộc")]
        [Range(1, 65535, ErrorMessage = "Cổng phải từ 1 đến 65535")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Tên đăng nhập SMTP")]
        [Required(ErrorMessage = "Tên đăng nhập SMTP là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không được vượt quá 100 ký tự")]
        public string SmtpUsername { get; set; } = "";

        [Display(Name = "Mật khẩu SMTP")]
        [Required(ErrorMessage = "Mật khẩu SMTP là bắt buộc")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
        public string SmtpPassword { get; set; } = "";

        [Display(Name = "Bật SSL/TLS")]
        public bool EnableSsl { get; set; } = true;

        [Display(Name = "Email gửi")]
        [Required(ErrorMessage = "Email gửi là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string FromEmail { get; set; } = "";

        [Display(Name = "Tên hiển thị")]
        [StringLength(100, ErrorMessage = "Tên hiển thị không được vượt quá 100 ký tự")]
        public string FromName { get; set; } = "";
    }
}