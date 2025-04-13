namespace DoAnChamSocSucKhoe.Models
{
    public class NguoiDung
    {
        public int NguoiDungId { get; set; }         // PK
        public int VaiTroId { get; set; }            // FK -> VaiTro

        public required string HoTen { get; set; }
        public required string Email { get; set; }
        public required string SoDienThoai { get; set; }
        public required string MatKhau { get; set; }
        public required string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public required string DiaChi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }          // Kích hoạt / Tạm khoá?

        // Navigation
        public required VaiTro VaiTro { get; set; }
        public required ICollection<DanhGiaChuyenGia> DanhGiaDaGui { get; set; }
        public required ICollection<ChuyenGia> ChuyenGias { get; set; }
    }
}
