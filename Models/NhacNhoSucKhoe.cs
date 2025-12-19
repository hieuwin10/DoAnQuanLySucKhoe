using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Models
{
    public class NhacNhoSucKhoe
    {
        [Key]
        public int NhacNhoSucKhoeId { get; set; }
        public string? UserId { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
        public bool DaThucHien { get; set; }
        public string LoaiNhacNho { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
