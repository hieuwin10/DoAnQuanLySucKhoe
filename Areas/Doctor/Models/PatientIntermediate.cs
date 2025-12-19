using System;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Models
{
    public class PatientIntermediate
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string ProfileImageUrl { get; set; }
        public DateTime LastVisit { get; set; }
        public required string LastDiagnosis { get; set; }
        public required string IdString { get; set; }
    }
}