using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class DanhGiaChuyenGia
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("NguoiDung")]
        public string? NguoiDungId { get; set; } // Thay đổi kiểu thành string
        public NguoiDung? NguoiDung { get; set; }

        public int? TuVanSucKhoeId { get; set; }
        [ForeignKey("TuVanSucKhoeId")]
        public TuVanSucKhoe? TuVanSucKhoe { get; set; }

        public string? ChuyenGiaId { get; set; }
        [ForeignKey("ChuyenGiaId")]
        public ChuyenGia? ChuyenGia { get; set; }

        // Các thuộc tính khác của DanhGiaChuyenGia
        public int? SoSao { get; set; }
        public string? BinhLuan { get; set; }
    }
}
