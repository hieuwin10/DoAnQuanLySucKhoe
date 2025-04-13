namespace DoAnChamSocSucKhoe.Models
{
    public class KeHoachTapLuyen
{
    public int KeHoachTapLuyenId { get; set; }   // PK
    public int NguoiDungId { get; set; }         // FK -> NguoiDung

    public required string TenKeHoach { get; set; }
    public required string MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public bool TrangThai { get; set; }          // Còn hiệu lực hay đã kết thúc?

    // Navigation
    public required NguoiDung NguoiDung { get; set; }
    public required ICollection<ChiTietKeHoachTapLuyen> ChiTietKeHoachTapLuyens { get; set; }
}
}
