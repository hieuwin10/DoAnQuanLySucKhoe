namespace DoAnChamSocSucKhoe.Models
{
    public class LichSuSucKhoe
{
    public int LichSuSucKhoeId { get; set; }     // PK
    public int NguoiDungId { get; set; }         // FK -> NguoiDung

    public decimal ChieuCao { get; set; }
    public decimal CanNang { get; set; }
    public int NhipTim { get; set; }
    public decimal DuongHuyet { get; set; }
    public decimal HuyetApTamThu { get; set; }
    public decimal HuyetApTamTruong { get; set; }
    public DateTime NgayDo { get; set; }         // Thời điểm đo/ghi nhận

    public string? GhiChu { get; set; }
    // Navigation
    public NguoiDung? NguoiDung { get; set; }
}
}
