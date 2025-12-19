using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class NguoiChamSocBenhNhan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? NguoiChamSocId { get; set; }

        [ForeignKey("NguoiChamSocId")]
        public NguoiDung? NguoiChamSoc { get; set; }

        [Required]
        public string? BenhNhanId { get; set; }

        [ForeignKey("BenhNhanId")]
        public NguoiDung? BenhNhan { get; set; }

        // Optional: Describe the relationship, e.g., "Family", "Nurse"
        public string? QuanHe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
