namespace DoAnChamSocSucKhoe.Areas.Patient.Models
{
    public class CreateAppointmentViewModel
    {
        public string? LoaiLichHen { get; set; }
        public string? ChuyenKhoa { get; set; }
        public required string ChuyenGiaId { get; set; }
        public required string NgayHen { get; set; } // String to handle format manually
        public required string GioHen { get; set; }
        public required string DiaDiem { get; set; }
        public required string LyDo { get; set; }
        public string? TrangThai { get; set; }
        public required string NguoiDungId { get; set; }
        public bool SuDungBHYT { get; set; } // Checkbox sends "true" or "on" if checked, nothing if not. MVC handles bool.
        public bool NhanNhacNho { get; set; }
    }
}
