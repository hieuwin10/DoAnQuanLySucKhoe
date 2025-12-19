using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class HoSoSucKhoe
    {
        public int HoSoSucKhoeId { get; set; }       // PK
        public required string NguoiDungId { get; set; }         // FK -> NguoiDung

        public decimal ChieuCao { get; set; }        // Đơn vị (cm)
        public decimal CanNang { get; set; }         // Đơn vị (kg)
        public int NhipTim { get; set; }             // Nhịp tim/phút
        public decimal DuongHuyet { get; set; }      // mg/dL (tuỳ)
        public decimal HuyetApTamThu { get; set; }   // mmHg
        public decimal HuyetApTamTruong { get; set; }// mmHg
        public DateTime NgayCapNhat { get; set; }
        public string? GhiChu { get; set; }

        // Additional properties needed by PatientsController
        public string? TrangThai { get; set; }       // Status (Đang điều trị, Cần theo dõi, Chờ khám, etc.)
        public DateTime? NgaySinh { get; set; }      // Birth date
        public string? GioiTinh { get; set; }        // Gender
        public string? ChanDoan { get; set; }        // Diagnosis
        public string? DiaChi { get; set; }          // Address
        public string? NhomMau { get; set; }         // Blood type
        public string? TienSuBenh { get; set; }      // Medical history
        public string? DiUng { get; set; }           // Allergies
        public string? ThuocDangDung { get; set; }   // Current medications
        public string? TienSuGiaDinh { get; set; }   // Family medical history
        public string? LoiSong { get; set; }         // Lifestyle
        public string? PhuongPhapDieuTri { get; set; } // Treatment methods

        // Navigation
        [ForeignKey("NguoiDungId")]
        public NguoiDung? NguoiDung { get; set; }
    }
}
