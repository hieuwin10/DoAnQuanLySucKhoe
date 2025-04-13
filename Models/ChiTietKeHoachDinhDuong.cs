namespace DoAnChamSocSucKhoe.Models
{
    public class ChiTietKeHoachDinhDuong
{
    public int ChiTietKeHoachDinhDuongId { get; set; } // PK
    public int KeHoachDinhDuongId { get; set; }        // FK -> KeHoachDinhDuong

    public required string MonAn { get; set; }
    public decimal SoLuong { get; set; }               // Lượng thực phẩm (gram, ml, ...)
    public required string MoTa { get; set; }
    public DateTime NgayThucHien { get; set; }
    public bool TrangThai { get; set; }                // Đã thực hiện / Chưa

    // Navigation
    public required KeHoachDinhDuong KeHoachDinhDuong { get; set; }
}
}
