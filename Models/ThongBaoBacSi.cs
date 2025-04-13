namespace DoAnChamSocSucKhoe.Models
{
    public class ThongBaoBacSi
{
    public int ThongBaoBacSiId { get; set; }    // PK
    public int NguoiDungId { get; set; }        // FK -> NguoiDung
    public int BacSiId { get; set; }            // FK -> NguoiDung (vai trò bác sĩ)

    public required string TieuDe { get; set; }
    public required string NoiDung { get; set; }
    public DateTime NgayTao { get; set; }
    public bool TrangThai { get; set; }         // Đã xem / Chưa xem?

    // Navigation
    public required NguoiDung NguoiDung { get; set; }
    public required NguoiDung BacSi { get; set; }
}
}
