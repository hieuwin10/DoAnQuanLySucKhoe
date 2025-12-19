using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Models
{
    public class DoctorProfileViewModel
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string? HoTen { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; } // Read-only

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Chuyên khoa")]
        [Required(ErrorMessage = "Vui lòng nhập chuyên khoa")]
        public string? ChuyenKhoa { get; set; }

        [Display(Name = "Chứng chỉ / Bằng cấp")]
        public string? ChungChi { get; set; }

        [Display(Name = "Kinh nghiệm")]
        public string? KinhNghiem { get; set; }

        [Display(Name = "Nơi công tác")]
        public string? NoiCongTac { get; set; }

        [Display(Name = "Mô tả / Giới thiệu")]
        public string? MoTa { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Tải ảnh lên")]
        public IFormFile? HinhAnhFile { get; set; }
        
        public bool TrangThai { get; set; }
    }
}
