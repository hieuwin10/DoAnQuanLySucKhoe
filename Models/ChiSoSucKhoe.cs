namespace DoAnChamSocSucKhoe.Models
{
    public class ChiSoSucKhoe
    {
        public int ChiSoSucKhoeId { get; set; }     // PK
        public int NguoiDungId { get; set; }        // FK -> NguoiDung
        public float ChieuCao { get; set; }         // cm
        public float CanNang { get; set; }          // kg
        public float BMI { get; set; }              // Chỉ số khối cơ thể
        public int NhipTim { get; set; }            // Nhịp/phút
        public int HuyetAp { get; set; }            // mmHg
        public float DuongHuyet { get; set; }       // mmol/L
        public DateTime NgayDo { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
    }
} 