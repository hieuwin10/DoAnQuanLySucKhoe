using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public int VaiTroId { get; set; }

        [Display(Name = "Tên vai trò")]
        public string TenVaiTro { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Số người dùng")]
        public int UserCount { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
    }

    public class RoleDetailViewModel
    {
        public int VaiTroId { get; set; }

        [Display(Name = "Tên vai trò")]
        public string TenVaiTro { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        [Display(Name = "Tên vai trò")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string TenVaiTro { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; } = string.Empty;
    }

    public class EditRoleViewModel
    {
        public int VaiTroId { get; set; }

        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        [Display(Name = "Tên vai trò")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string TenVaiTro { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; } = string.Empty;
    }
}