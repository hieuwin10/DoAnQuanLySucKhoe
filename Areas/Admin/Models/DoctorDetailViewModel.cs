using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class DoctorDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public required string Status { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastActivity { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public string GetStatusClass() => DoctorDetailViewModel.GetStatusClass(Status);
        public string GetCompletionRate() => DoctorDetailViewModel.GetCompletionRate(CompletedAppointments, TotalAppointments);

        public static string GetStatusClass(string status) => status switch
        {
            "Hoạt động" => "success",
            "Chờ duyệt" => "warning",
            "Không hoạt động" => "danger",
            _ => "secondary"
        };

        public static string GetCompletionRate(int completed, int total) => total > 0
            ? $"{(double)completed / total * 100:F1}%"
            : "0%";
    }

    public class CreateDoctorViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Chuyên môn")]
        [StringLength(200, ErrorMessage = "Chuyên môn không được vượt quá 200 ký tự")]
        public string? Specialization { get; set; }

        [Display(Name = "Số năm kinh nghiệm")]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int ExperienceYears { get; set; }

        [Display(Name = "Tiểu sử")]
        [StringLength(1000, ErrorMessage = "Tiểu sử không được vượt quá 1000 ký tự")]
        public string? Bio { get; set; }

        [Display(Name = "Học vấn")]
        [StringLength(500, ErrorMessage = "Học vấn không được vượt quá 500 ký tự")]
        public string? Education { get; set; }

        [Display(Name = "Chứng chỉ")]
        [StringLength(500, ErrorMessage = "Chứng chỉ không được vượt quá 500 ký tự")]
        public string? Certifications { get; set; }
    }

    public class EditDoctorViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Chuyên môn")]
        [StringLength(200, ErrorMessage = "Chuyên môn không được vượt quá 200 ký tự")]
        public string? Specialization { get; set; }

        [Display(Name = "Số năm kinh nghiệm")]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int ExperienceYears { get; set; }

        [Display(Name = "Tiểu sử")]
        [StringLength(1000, ErrorMessage = "Tiểu sử không được vượt quá 1000 ký tự")]
        public string? Bio { get; set; }

        [Display(Name = "Học vấn")]
        [StringLength(500, ErrorMessage = "Học vấn không được vượt quá 500 ký tự")]
        public string? Education { get; set; }

        [Display(Name = "Chứng chỉ")]
        [StringLength(500, ErrorMessage = "Chứng chỉ không được vượt quá 500 ký tự")]
        public string? Certifications { get; set; }
    }
}