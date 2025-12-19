using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class SettingsViewModel
    {
        [Display(Name = "Tên hệ thống")]
        [Required(ErrorMessage = "Tên hệ thống là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên hệ thống không được vượt quá 100 ký tự")]
        public string SystemName { get; set; } = "Hệ thống chăm sóc sức khỏe";

        [Display(Name = "Mô tả hệ thống")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string SystemDescription { get; set; } = "Hệ thống quản lý và tư vấn chăm sóc sức khỏe";

        [Display(Name = "Email quản trị")]
        [Required(ErrorMessage = "Email quản trị là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string AdminEmail { get; set; } = "admin@healthcare.com";

        [Display(Name = "Số điện thoại hỗ trợ")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SupportPhone { get; set; } = "";

        [Display(Name = "Email hỗ trợ")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string SupportEmail { get; set; } = "";

        [Display(Name = "Địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; } = "";

        [Display(Name = "Số lượng mục trên mỗi trang")]
        [Range(5, 100, ErrorMessage = "Số lượng phải từ 5 đến 100")]
        public int ItemsPerPage { get; set; } = 10;

        [Display(Name = "Cho phép đăng ký tài khoản")]
        public bool AllowRegistration { get; set; } = true;

        [Display(Name = "Yêu cầu xác thực email")]
        public bool RequireEmailConfirmation { get; set; } = false;

        [Display(Name = "Cho phép đăng nhập bằng Google")]
        public bool EnableGoogleLogin { get; set; } = false;

        [Display(Name = "Cho phép đăng nhập bằng Facebook")]
        public bool EnableFacebookLogin { get; set; } = false;

        [Display(Name = "Gửi email thông báo")]
        public bool SendNotificationEmails { get; set; } = true;

        [Display(Name = "Gửi email nhắc nhở")]
        public bool SendReminderEmails { get; set; } = true;

        [Display(Name = "Thời gian lưu phiên (phút)")]
        [Range(15, 1440, ErrorMessage = "Thời gian phải từ 15 phút đến 24 giờ")]
        public int SessionTimeoutMinutes { get; set; } = 60;

        [Display(Name = "Ngôn ngữ mặc định")]
        public string DefaultLanguage { get; set; } = "vi";

        [Display(Name = "Múi giờ")]
        public string TimeZone { get; set; } = "SE Asia Standard Time";
    }
}