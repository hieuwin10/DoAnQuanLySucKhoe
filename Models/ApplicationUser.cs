using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [PersonalData]
        [Column("ho_ten")]
        public string? HoTen { get; set; }

        [PersonalData]
        [Column("gioi_tinh")]
        public string? GioiTinh { get; set; }

        [PersonalData]
        [Column("ngay_sinh")]
        public DateTime? NgaySinh { get; set; }

        [PersonalData]
        [Column("dia_chi")]
        public string? DiaChi { get; set; }

        [Column("anh_dai_dien")]
        public string? ProfilePicture { get; set; }

        [Required]
        [Column("vai_tro_id")]
        public int VaiTroId { get; set; }

        [Column("ngay_tao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("VaiTroId")]
        public virtual VaiTro? VaiTro { get; set; }
    }
}