namespace DoAnChamSocSucKhoe.Models
{
    public class HoSoSucKhoe
{
    public int HoSoSucKhoeId { get; set; }       // PK
    public int NguoiDungId { get; set; }         // FK -> NguoiDung

    public decimal ChieuCao { get; set; }        // Đơn vị (cm)
    public decimal CanNang { get; set; }         // Đơn vị (kg)
    public int NhipTim { get; set; }             // Nhịp tim/phút
    public decimal DuongHuyet { get; set; }      // mg/dL (tuỳ)
    public decimal HuyetApTamThu { get; set; }   // mmHg
    public decimal HuyetApTamTruong { get; set; }// mmHg
    public DateTime NgayCapNhat { get; set; }
    public string? GhiChu { get; set; }

    // Navigation
    public NguoiDung? NguoiDung { get; set; }
}
}
