namespace DoAnChamSocSucKhoe.Models
{
    public class VaiTro
    {
        public int VaiTroId { get; set; }            // PK
        public required string TenVaiTro { get; set; }        // Tên vai trò (VD: Bác sĩ, Chuyên gia, Khách hàng...)
        public required string MoTa { get; set; }             // Mô tả thêm (nếu cần)
        public DateTime NgayTao { get; set; } = DateTime.Now; // Ngày tạo vai trò

        // Navigation
        public required ICollection<NguoiDung> NguoiDungs { get; set; }
    }
}
