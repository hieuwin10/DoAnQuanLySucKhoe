using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class FileHoSo
    {
        public int FileHoSoId { get; set; }
        public int HoSoSucKhoeId { get; set; }
        public string? TenFile { get; set; }
        public string? DuongDan { get; set; }
        public string? LoaiFile { get; set; }
        public long KichThuoc { get; set; }
        public DateTime NgayTaiLen { get; set; }
        public string? NguoiTaiLenId { get; set; }
        public string? MoTa { get; set; }
        
        [ForeignKey("HoSoSucKhoeId")]
        public HoSoSucKhoe? HoSoSucKhoe { get; set; }
        
        [ForeignKey("NguoiTaiLenId")]
        public NguoiDung? NguoiTaiLen { get; set; }
    }
}
