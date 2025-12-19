namespace DoAnChamSocSucKhoe.Models
{
    public class KeHoachTapLuyen
    {
        public int KeHoachTapLuyenId { get; set; }   // PK
        public int Id { get { return KeHoachTapLuyenId; } set { KeHoachTapLuyenId = value; } } // Alias for KeHoachTapLuyenId
        public required string NguoiDungId { get; set; }  // FK -> NguoiDung - Changed from int to string

        public required string TenKeHoach { get; set; }
        public required string MoTa { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }          // Còn hiệu lực hay đã kết thúc?

        // Navigation
        public NguoiDung? NguoiDung { get; set; }  // Changed to nullable to avoid required attribute
        public required ICollection<ChiTietKeHoachTapLuyen> ChiTietKeHoachTapLuyens { get; set; }
    }
}
