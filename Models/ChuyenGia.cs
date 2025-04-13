namespace DoAnChamSocSucKhoe.Models
{
    public class ChuyenGia
    {
        public int ChuyenGiaId { get; set; }         // PK
        public int NguoiDungId { get; set; }         // FK -> NguoiDung
        public required string ChuyenKhoa { get; set; }
        public required string ChungChi { get; set; }
        public required string KinhNghiem { get; set; }
        public required string NoiCongTac { get; set; }
        public required string MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }          // Đang hoạt động / Tạm nghỉ?

        // Navigation
        public required NguoiDung NguoiDung { get; set; }
        public required ICollection<DanhGiaChuyenGia> DanhGiaDaNhan { get; set; }
        public required ICollection<TuVanSucKhoe> TuVanSucKhoes { get; set; }
    }
} 