namespace DoAnChamSocSucKhoe.Models
{
    public class ChiSoSucKhoe
    {
        public int ChiSoSucKhoeId { get; set; }     // PK
        public required string NguoiDungId { get; set; }     // FK -> NguoiDung (changed from int to string to match NguoiDung.Id)
        public float ChieuCao { get; set; }         // cm
        public float CanNang { get; set; }          // kg
        public float BMI { get; set; }              // Chỉ số khối cơ thể
        public int NhipTim { get; set; }            // Nhịp/phút
        public int HuyetAp { get; set; }            // mmHg
        public float DuongHuyet { get; set; }       // mmol/L
        public DateTime NgayDo { get; set; }
        public DateTime NgayCapNhat { get; set; }   // Added for compatibility with PatientsController
        public string? GhiChu { get; set; }

        // Properties needed by PatientsController
        public required string LoaiChiSo { get; set; } // Type of health metric
        public required string GiaTri { get; set; }    // Value as string for display

        // Navigation
        public NguoiDung? NguoiDung { get; set; }   // Changed to nullable to avoid required attribute
    }
}