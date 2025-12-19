using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Models
{
    public class NguoiDung : IdentityUser
    {
        // Các thuộc tính Id, UserName, Email, PhoneNumber đã được kế thừa từ IdentityUser.
        // Mật khẩu (PasswordHash) cũng được quản lý bởi Identity.

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Vai trò")]
        public int? VaiTroId { get; set; } // Khóa ngoại liên kết với VaiTro
        public VaiTro? VaiTro { get; set; } // Thuộc tính điều hướng đến VaiTro

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang hoạt động";

        [Display(Name = "Ảnh đại diện")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDien { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        public ICollection<DanhGiaChuyenGia>? DanhGiaDaGui { get; set; }
        public ICollection<ChuyenGia>? ChuyenGias { get; set; }

        // Navigation properties
        // Quan hệ 1-1 với HoSoSucKhoe
        public HoSoSucKhoe? HoSoSucKhoe { get; set; }

        // Add these collections referenced in PatientsController
        public ICollection<ChiSoSucKhoe>? ChiSoSucKhoes { get; set; }
        public ICollection<LichHen>? LichHens { get; set; }
    }
}
