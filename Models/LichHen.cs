namespace DoAnChamSocSucKhoe.Models
{
    public class LichHen
    {
        public int LichHenId { get; set; }          // PK
        public int Id { get { return LichHenId; } set { LichHenId = value; } } // Alias for LichHenId to fix PatientsController reference
        public required string NguoiDungId { get; set; }        // FK -> NguoiDung (patient)
        public required string ChuyenGiaId { get; set; }        // FK -> NguoiDung (người dùng có vai trò chuyên gia/bác sĩ)

        public DateTime NgayGioHen { get; set; }
        public DateTime NgayHen { get; set; } // Added for compatibility with PatientsController
        public required string DiaDiem { get; set; }
        public required string LyDo { get; set; }
        public string? LoaiLichHen { get; set; } // Online/Offline
        public required string TrangThai { get; set; }       // Đã xác nhận / Chờ xác nhận / Đã hủy?
        public string? ChanDoan { get; set; } // Added for compatibility with PatientsController
        public string? DonThuoc { get; set; } // Added for compatibility with PatientsController
        public string? GhiChu { get; set; } // Added for compatibility with PatientsController

        // Navigation
        public NguoiDung? NguoiDung { get; set; } // Patient
        public NguoiDung? ChuyenGia { get; set; } // Doctor
        public NguoiDung? BacSi { get; set; } // Alias for ChuyenGia for compatibility with PatientsController
        public string? BenhNhanId { get; set; } // Added for compatibility with PatientsController
    }
}
