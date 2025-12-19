using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class BackupSettingsViewModel
    {
        [Display(Name = "Bật sao lưu tự động")]
        public bool AutoBackupEnabled { get; set; } = true;

        [Display(Name = "Tần suất sao lưu")]
        [Required(ErrorMessage = "Tần suất sao lưu là bắt buộc")]
        public string BackupFrequency { get; set; } = "daily";

        [Display(Name = "Thời gian sao lưu")]
        [Required(ErrorMessage = "Thời gian sao lưu là bắt buộc")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Thời gian phải có định dạng HH:MM")]
        public string BackupTime { get; set; } = "02:00";

        [Display(Name = "Số ngày lưu trữ")]
        [Range(1, 365, ErrorMessage = "Số ngày lưu trữ phải từ 1 đến 365")]
        public int RetentionDays { get; set; } = 30;

        [Display(Name = "Vị trí lưu trữ")]
        [Required(ErrorMessage = "Vị trí lưu trữ là bắt buộc")]
        [StringLength(500, ErrorMessage = "Vị trí lưu trữ không được vượt quá 500 ký tự")]
        public string BackupLocation { get; set; } = "/backups/";

        // Read-only properties for display
        public DateTime? LastBackupDate { get; set; }
        public string LastBackupStatus { get; set; } = "Thành công";
    }
}