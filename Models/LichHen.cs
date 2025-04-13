namespace DoAnChamSocSucKhoe.Models
{
    public class LichHen
{
    public int LichHenId { get; set; }          // PK
    public int NguoiDungId { get; set; }        // FK -> NguoiDung
    public int ChuyenGiaId { get; set; }        // FK -> NguoiDung (người dùng có vai trò chuyên gia?)

    public DateTime NgayGioHen { get; set; }
    public required string DiaDiem { get; set; }
    public required string LyDo { get; set; }
    public required string TrangThai { get; set; }       // Đã xác nhận / Chờ xác nhận / Đã hủy?

    // Navigation
    public required NguoiDung NguoiDung { get; set; }
    public required NguoiDung ChuyenGia { get; set; }
}
}
