using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class DanhGiaChuyenGia
    {
        [Key]
        public int DanhGiaChuyenGiaId { get; set; }

        [ForeignKey("NguoiDung")]
        public int NguoiDungId { get; set; }

        [ForeignKey("ChuyenGia")]
        public int ChuyenGiaId { get; set; }

        public required string NoiDung { get; set; }
        public int DiemSo { get; set; }
        public DateTime NgayDanhGia { get; set; }

        public required NguoiDung NguoiDung { get; set; }
        public required ChuyenGia ChuyenGia { get; set; }
    }
}
