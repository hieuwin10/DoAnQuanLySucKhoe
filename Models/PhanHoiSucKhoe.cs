namespace DoAnChamSocSucKhoe.Models
{
    public class PhanHoiSucKhoe
{
    public int PhanHoiSucKhoeId { get; set; }   // PK
    public int NguoiDungId { get; set; }        // FK -> NguoiDung

    public required string NoiDung { get; set; }
    public DateTime NgayTao { get; set; }
    public bool TrangThai { get; set; }         // Đã phản hồi xong? Đã xử lý?

    // Navigation
    public required NguoiDung NguoiDung { get; set; }
}
}
