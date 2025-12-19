using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnChamSocSucKhoe.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAllNonAdminUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Create a temporary table to store IDs of users to delete
                DECLARE @UsersToDelete TABLE (Id NVARCHAR(450));

                -- Insert IDs of all users EXCEPT the admin
                INSERT INTO @UsersToDelete (Id)
                SELECT Id FROM AspNetUsers WHERE Email <> 'admin@healthmanager.com';

                -- Delete related data for these users
                
                -- 1. NguoiChamSocBenhNhans (Relationships)
                DELETE FROM NguoiChamSocBenhNhans WHERE NguoiChamSocId IN (SELECT Id FROM @UsersToDelete) OR BenhNhanId IN (SELECT Id FROM @UsersToDelete);

                -- 2. Messages
                DELETE FROM Messages WHERE SenderId IN (SELECT Id FROM @UsersToDelete) OR ReceiverId IN (SELECT Id FROM @UsersToDelete);

                -- 3. TuVanSucKhoes (Consultations)
                DELETE FROM TuVanSucKhoes WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete) OR ChuyenGiaId IN (SELECT Id FROM @UsersToDelete);

                -- 4. LichHens (Appointments)
                DELETE FROM LichHens WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete) OR ChuyenGiaId IN (SELECT Id FROM @UsersToDelete) OR BacSiId IN (SELECT Id FROM @UsersToDelete) OR BenhNhanId IN (SELECT Id FROM @UsersToDelete);

                -- 5. HoSoSucKhoes (Health Profiles)
                DELETE FROM HoSoSucKhoes WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 6. ChiSoSucKhoes (Health Indices)
                DELETE FROM ChiSoSucKhoes WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 7. NhacNhoSucKhoes (Reminders)
                DELETE FROM NhacNhoSucKhoes WHERE UserId IN (SELECT Id FROM @UsersToDelete);

                -- 8. KeHoachDinhDuongs (Nutrition Plans)
                DELETE FROM ChiTietKeHoachDinhDuongs WHERE KeHoachDinhDuongId IN (SELECT KeHoachDinhDuongId FROM KeHoachDinhDuongs WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete));
                DELETE FROM KeHoachDinhDuongs WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 9. KeHoachTapLuyens (Exercise Plans)
                DELETE FROM ChiTietKeHoachTapLuyens WHERE KeHoachTapLuyenId IN (SELECT KeHoachTapLuyenId FROM KeHoachTapLuyens WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete));
                DELETE FROM KeHoachTapLuyens WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 10. DanhGiaChuyenGias (Reviews)
                DELETE FROM DanhGiaChuyenGias WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete) OR ChuyenGiaId IN (SELECT Id FROM @UsersToDelete);

                -- 11. ChuyenGias (Doctor Profiles)
                DELETE FROM ChuyenGias WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 12. ThongBaoBacSis (Notifications)
                DELETE FROM ThongBaoBacSis WHERE BacSiId IN (SELECT Id FROM @UsersToDelete) OR NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 13. FileHoSos
                DELETE FROM FileHoSos WHERE NguoiTaiLenId IN (SELECT Id FROM @UsersToDelete);

                -- 14. LichSuHoSoSucKhoes
                DELETE FROM LichSuHoSoSucKhoes WHERE NguoiThayDoiId IN (SELECT Id FROM @UsersToDelete);

                -- 15. LichSuSucKhoes
                DELETE FROM LichSuSucKhoes WHERE NguoiDungId IN (SELECT Id FROM @UsersToDelete);

                -- 16. Identity Tables
                DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM @UsersToDelete);
                DELETE FROM AspNetUserClaims WHERE UserId IN (SELECT Id FROM @UsersToDelete);
                DELETE FROM AspNetUserLogins WHERE UserId IN (SELECT Id FROM @UsersToDelete);
                DELETE FROM AspNetUserTokens WHERE UserId IN (SELECT Id FROM @UsersToDelete);

                -- Finally, delete the users
                DELETE FROM AspNetUsers WHERE Id IN (SELECT Id FROM @UsersToDelete);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
