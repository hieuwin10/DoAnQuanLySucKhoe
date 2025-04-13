namespace DoAnChamSocSucKhoe.Models
{
    public class TuVanSucKhoe
    {
        public int TuVanSucKhoeId { get; set; }      // PK
        public int NguoiDungId { get; set; }         // FK -> NguoiDung
        public int ChuyenGiaId { get; set; }         // FK -> ChuyenGia
        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public required string TraLoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int TrangThai { get; set; }           // Chờ tư vấn / Đã tư vấn / Huỷ

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
        public required ChuyenGia ChuyenGia { get; set; }
    }
} 