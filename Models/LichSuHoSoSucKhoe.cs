using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class LichSuHoSoSucKhoe
    {
        public int LichSuHoSoSucKhoeId { get; set; }
        public int HoSoSucKhoeId { get; set; }
        public string? NguoiThayDoiId { get; set; }
        public DateTime NgayThayDoi { get; set; }
        public string? ThayDoiNoiDung { get; set; }
        public string? LoaiThayDoi { get; set; } // "Tạo mới", "Cập nhật", "Bác sĩ cập nhật"
        
        [ForeignKey("HoSoSucKhoeId")]
        public HoSoSucKhoe? HoSoSucKhoe { get; set; }
        
        [ForeignKey("NguoiThayDoiId")]
        public NguoiDung? NguoiThayDoi { get; set; }
    }
}
