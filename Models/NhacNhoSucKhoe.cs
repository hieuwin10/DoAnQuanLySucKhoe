namespace DoAnChamSocSucKhoe.Models
{
    public class NhacNhoSucKhoe
    {
        public int NhacNhoSucKhoeId { get; set; }    // PK
        public int NguoiDungId { get; set; }         // FK -> NguoiDung
        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public DateTime ThoiGianNhac { get; set; }
        public bool LapLai { get; set; }             // Có lặp lại không?
        public string? ChuKyLap { get; set; }        // Hàng ngày/tuần/tháng (nếu có)
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }          // Đang kích hoạt / Đã tắt

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
    }
} 