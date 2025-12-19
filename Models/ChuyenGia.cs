namespace DoAnChamSocSucKhoe.Models
{
    public class ChuyenGia
    {
        public required string ChuyenGiaId { get; set; }         // PK
        public required string NguoiDungId { get; set; }         // FK -> NguoiDung
        public required string ChuyenKhoa { get; set; }
        public required string ChungChi { get; set; }
        public required string KinhNghiem { get; set; }
        public required string NoiCongTac { get; set; }
        public required string MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }          // Đang hoạt động / Tạm nghỉ?
        public string? HinhAnh { get; set; }         // Ảnh chuyên gia
        public string? HoTen { get; set; }           // Họ tên chuyên gia

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
        public required ICollection<DanhGiaChuyenGia> DanhGiaDaNhan { get; set; }
        public required ICollection<TuVanSucKhoe> TuVanSucKhoes { get; set; }
    }
}