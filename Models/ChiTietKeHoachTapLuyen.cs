namespace DoAnChamSocSucKhoe.Models
{
    public class ChiTietKeHoachTapLuyen
    {
        public int ChiTietKeHoachTapLuyenId { get; set; }  // PK
        public int KeHoachTapLuyenId { get; set; }         // FK -> KeHoachTapLuyen
        public required string NguoiDungId { get; set; }               // FK -> NguoiDung

        public required string BaiTap { get; set; }        // Tên bài tập
        public int SoLanLap { get; set; }                  // Số lần lặp
        public int ThoiGian { get; set; }                  // Phút hoặc giây
        public required string MoTa { get; set; }
        public DateTime NgayThucHien { get; set; }
        public bool TrangThai { get; set; }                // Hoàn thành hay chưa?

        // Navigation properties
        public required KeHoachTapLuyen KeHoachTapLuyen { get; set; }
        public required NguoiDung NguoiDung { get; set; }
    }
}
