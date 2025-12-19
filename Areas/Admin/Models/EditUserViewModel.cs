using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Vai trò")]
        public int RoleId { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; } = string.Empty;

        [Display(Name = "Thay đổi ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string? ConfirmPassword { get; set; }

        // Quyền hạn
        public bool CanViewProfile { get; set; } = true;
        public bool CanBookAppointment { get; set; } = false;
        public bool CanSendConsultation { get; set; } = false;
        public bool CanViewMedicalHistory { get; set; } = false;
        public bool CanManageAccounts { get; set; } = false;

        [Display(Name = "Bệnh nhân được chăm sóc")]
        public string? SelectedPatientId { get; set; }
    }
}