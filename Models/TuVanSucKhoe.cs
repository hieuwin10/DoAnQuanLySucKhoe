namespace DoAnChamSocSucKhoe.Models
{
    public class TuVanSucKhoe
    {
        public int TuVanSucKhoeId { get; set; }      // PK
        public required string NguoiDungId { get; set; }         // FK -> NguoiDung
        public required string ChuyenGiaId { get; set; }         // FK -> ChuyenGia
        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public string? TraLoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayTraLoi { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int TrangThai { get; set; }           // Chờ tư vấn / Đã tư vấn / Huỷ

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
        public required ChuyenGia ChuyenGia { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public DanhGiaChuyenGia? DanhGia { get; set; }
    }
}