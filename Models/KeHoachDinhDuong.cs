namespace DoAnChamSocSucKhoe.Models
{
    public class KeHoachDinhDuong
    {
        public int KeHoachDinhDuongId { get; set; }  // PK
        public int Id { get { return KeHoachDinhDuongId; } set { KeHoachDinhDuongId = value; } } // Alias for KeHoachDinhDuongId
        public required string NguoiDungId { get; set; }  // FK -> NguoiDung - Changed from int to string

        public required string TenKeHoach { get; set; }
        public required string MoTa { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }

        // Navigation
        public NguoiDung? NguoiDung { get; set; }  // Changed to nullable to avoid required attribute
        public required ICollection<ChiTietKeHoachDinhDuong> ChiTietKeHoachDinhDuongs { get; set; }
    }
}
