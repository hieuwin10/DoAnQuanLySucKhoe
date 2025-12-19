namespace DoAnChamSocSucKhoe.Models
{
    public class ThongBaoBacSi
    {
        public int ThongBaoBacSiId { get; set; }    // PK
        public required string NguoiDungId { get; set; }  // FK -> NguoiDung - Changed from int to string
        public required string BacSiId { get; set; }      // FK -> NguoiDung (vai trò bác sĩ) - Changed from int to string

        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; }         // Đã xem / Chưa xem?

        // Navigation
        public NguoiDung? NguoiDung { get; set; }   // Changed to nullable to avoid required attribute
        public NguoiDung? BacSi { get; set; }       // Changed to nullable to avoid required attribute
    }
}
