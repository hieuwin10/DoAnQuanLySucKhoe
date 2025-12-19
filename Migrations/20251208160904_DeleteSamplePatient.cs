using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnChamSocSucKhoe.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSamplePatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @UserId NVARCHAR(450);
                SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'patient@example.com';

                IF @UserId IS NOT NULL
                BEGIN
                    DELETE FROM HoSoSucKhoes WHERE NguoiDungId = @UserId;
                    DELETE FROM LichHens WHERE BenhNhanId = @UserId;
                    DELETE FROM TuVanSucKhoes WHERE NguoiDungId = @UserId;
                    DELETE FROM Messages WHERE SenderId = @UserId OR ReceiverId = @UserId;
                    DELETE FROM AspNetUserRoles WHERE UserId = @UserId;
                    DELETE FROM AspNetUsers WHERE Id = @UserId;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
