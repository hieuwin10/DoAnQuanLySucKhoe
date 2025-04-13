using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần cho [ForeignKey] nếu dùng

namespace YourProjectName.Models // Thay YourProjectName bằng namespace dự án của bạn
{
    public class HealthMetric
    {
        public int Id { get; set; }

        // Khóa ngoại tới bảng Users (ví dụ: AspNetUsers)
        [Required]
        public string PatientId { get; set; }

        // Nếu dùng EF Core, bạn có thể thêm navigation property (tùy chọn)
        // public virtual ApplicationUser Patient { get; set; } // Thay ApplicationUser bằng lớp User của bạn

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày ghi nhận")]
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow; // Mặc định là ngày giờ hiện tại

        [Range(1, 500, ErrorMessage = "Cân nặng không hợp lệ (1-500 kg).")]
        [Display(Name = "Cân nặng (kg)")]
        public double? Weight { get; set; } // Dùng nullable double/int để không bắt buộc nhập tất cả

        [Range(50, 300, ErrorMessage = "Chiều cao không hợp lệ (50-300 cm).")]
        [Display(Name = "Chiều cao (cm)")]
        public double? Height { get; set; }

        [Range(30, 300, ErrorMessage = "Huyết áp tâm thu không hợp lệ.")]
        [Display(Name = "Huyết áp tâm thu (mmHg)")]
        public int? SystolicPressure { get; set; }

        [Range(30, 200, ErrorMessage = "Huyết áp tâm trương không hợp lệ.")]
        [Display(Name = "Huyết áp tâm trương (mmHg)")]
        public int? DiastolicPressure { get; set; }

        [Range(30, 600, ErrorMessage = "Đường huyết không hợp lệ.")]
        [Display(Name = "Đường huyết (mg/dL)")] // Hoặc mmol/L tùy đơn vị
        public double? BloodSugar { get; set; }

        [Range(30, 250, ErrorMessage = "Nhịp tim không hợp lệ.")]
        [Display(Name = "Nhịp tim (bpm)")]
        public int? HeartRate { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        // Bạn có thể thêm các thuộc tính khác như BMI (tính toán), Cholesterol, etc.
    }
}