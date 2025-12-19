using DoAnChamSocSucKhoe.Models;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Models
{
    public class HealthRecordDetailViewModel
    {
        public NguoiDung? Patient { get; set; }
        public HoSoSucKhoe? HealthProfile { get; set; }
        public IEnumerable<ChiSoSucKhoe> HealthMetrics { get; set; } = new List<ChiSoSucKhoe>();
        public IEnumerable<LichSuSucKhoe> HealthHistories { get; set; } = new List<LichSuSucKhoe>();
        public IEnumerable<KeHoachDinhDuong> NutritionPlans { get; set; } = new List<KeHoachDinhDuong>();
        public IEnumerable<KeHoachTapLuyen> ExercisePlans { get; set; } = new List<KeHoachTapLuyen>();
        public IEnumerable<NhacNhoSucKhoe> HealthReminders { get; set; } = new List<NhacNhoSucKhoe>();
    }
}
