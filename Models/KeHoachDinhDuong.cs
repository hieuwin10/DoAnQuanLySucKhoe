namespace DoAnChamSocSucKhoe.Models
{
    public class KeHoachDinhDuong
{
    public int KeHoachDinhDuongId { get; set; }  // PK
    public int NguoiDungId { get; set; }         // FK -> NguoiDung

    public required string TenKeHoach { get; set; }
    public required string MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public bool TrangThai { get; set; }

    // Navigation
    public required NguoiDung NguoiDung { get; set; }
    public required ICollection<ChiTietKeHoachDinhDuong> ChiTietKeHoachDinhDuongs { get; set; }
}
}
