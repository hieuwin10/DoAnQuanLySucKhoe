using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnChamSocSucKhoe.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    VaiTroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.VaiTroId);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    NguoiDungId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaiTroId = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.NguoiDungId);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_VaiTros_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "VaiTros",
                        principalColumn: "VaiTroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoSucKhoe",
                columns: table => new
                {
                    ChiSoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChieuCao = table.Column<float>(type: "real", nullable: false),
                    CanNang = table.Column<float>(type: "real", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    HuyetAp = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<float>(type: "real", nullable: false),
                    NgayDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoSucKhoe", x => x.ChiSoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_ChiSoSucKhoe_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenGia",
                columns: table => new
                {
                    ChuyenGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChungChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KinhNghiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiCongTac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenGia", x => x.ChuyenGiaId);
                    table.ForeignKey(
                        name: "FK_ChuyenGia_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId");
                });

            migrationBuilder.CreateTable(
                name: "HoSoSucKhoes",
                columns: table => new
                {
                    HoSoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChieuCao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CanNang = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HuyetApTamThu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HuyetApTamTruong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoSucKhoes", x => x.HoSoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_HoSoSucKhoes_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachDinhDuongs",
                columns: table => new
                {
                    KeHoachDinhDuongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachDinhDuongs", x => x.KeHoachDinhDuongId);
                    table.ForeignKey(
                        name: "FK_KeHoachDinhDuongs_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachTapLuyens",
                columns: table => new
                {
                    KeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachTapLuyens", x => x.KeHoachTapLuyenId);
                    table.ForeignKey(
                        name: "FK_KeHoachTapLuyens_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHens",
                columns: table => new
                {
                    LichHenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChuyenGiaId = table.Column<int>(type: "int", nullable: false),
                    NgayGioHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHens", x => x.LichHenId);
                    table.ForeignKey(
                        name: "FK_LichHens_NguoiDungs_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHens_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId");
                });

            migrationBuilder.CreateTable(
                name: "LichSuSucKhoes",
                columns: table => new
                {
                    LichSuSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChieuCao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CanNang = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HuyetApTamThu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HuyetApTamTruong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuSucKhoes", x => x.LichSuSucKhoeId);
                    table.ForeignKey(
                        name: "FK_LichSuSucKhoes_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhacNhoSucKhoe",
                columns: table => new
                {
                    NhacNhoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianNhac = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LapLai = table.Column<bool>(type: "bit", nullable: false),
                    ChuKyLap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhacNhoSucKhoe", x => x.NhacNhoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_NhacNhoSucKhoe_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanHoiSucKhoes",
                columns: table => new
                {
                    PhanHoiSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoiSucKhoes", x => x.PhanHoiSucKhoeId);
                    table.ForeignKey(
                        name: "FK_PhanHoiSucKhoes_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaoBacSis",
                columns: table => new
                {
                    ThongBaoBacSiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    BacSiId = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoBacSis", x => x.ThongBaoBacSiId);
                    table.ForeignKey(
                        name: "FK_ThongBaoBacSis_NguoiDungs_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId");
                    table.ForeignKey(
                        name: "FK_ThongBaoBacSis_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId");
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaChuyenGias",
                columns: table => new
                {
                    DanhGiaChuyenGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChuyenGiaId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemSo = table.Column<int>(type: "int", nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaChuyenGias", x => x.DanhGiaChuyenGiaId);
                    table.ForeignKey(
                        name: "FK_DanhGiaChuyenGias_ChuyenGia_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "ChuyenGia",
                        principalColumn: "ChuyenGiaId");
                    table.ForeignKey(
                        name: "FK_DanhGiaChuyenGias_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId");
                });

            migrationBuilder.CreateTable(
                name: "TuVanSucKhoes",
                columns: table => new
                {
                    TuVanSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    ChuyenGiaId = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraLoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuVanSucKhoes", x => x.TuVanSucKhoeId);
                    table.ForeignKey(
                        name: "FK_TuVanSucKhoes_ChuyenGia_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "ChuyenGia",
                        principalColumn: "ChuyenGiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuVanSucKhoes_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKeHoachDinhDuongs",
                columns: table => new
                {
                    ChiTietKeHoachDinhDuongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeHoachDinhDuongId = table.Column<int>(type: "int", nullable: false),
                    MonAn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKeHoachDinhDuongs", x => x.ChiTietKeHoachDinhDuongId);
                    table.ForeignKey(
                        name: "FK_ChiTietKeHoachDinhDuongs_KeHoachDinhDuongs_KeHoachDinhDuongId",
                        column: x => x.KeHoachDinhDuongId,
                        principalTable: "KeHoachDinhDuongs",
                        principalColumn: "KeHoachDinhDuongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKeHoachTapLuyens",
                columns: table => new
                {
                    ChiTietKeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false),
                    BaiTap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLanLap = table.Column<int>(type: "int", nullable: false),
                    ThoiGian = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKeHoachTapLuyens", x => x.ChiTietKeHoachTapLuyenId);
                    table.ForeignKey(
                        name: "FK_ChiTietKeHoachTapLuyens_KeHoachTapLuyens_KeHoachTapLuyenId",
                        column: x => x.KeHoachTapLuyenId,
                        principalTable: "KeHoachTapLuyens",
                        principalColumn: "KeHoachTapLuyenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "67586ba7-5328-43a1-91ea-c4cab88444d6", "Admin", "ADMIN" },
                    { "2", "6e6e1cfb-2785-482f-bde0-96d74de081ca", "Doctor", "DOCTOR" },
                    { "3", "8e4818eb-f3c1-4979-8307-4e8666b76a94", "Patient", "PATIENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoSucKhoe_NguoiDungId",
                table: "ChiSoSucKhoe",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKeHoachDinhDuongs_KeHoachDinhDuongId",
                table: "ChiTietKeHoachDinhDuongs",
                column: "KeHoachDinhDuongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKeHoachTapLuyens_KeHoachTapLuyenId",
                table: "ChiTietKeHoachTapLuyens",
                column: "KeHoachTapLuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenGia_NguoiDungId",
                table: "ChuyenGia",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaChuyenGias_ChuyenGiaId",
                table: "DanhGiaChuyenGias",
                column: "ChuyenGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaChuyenGias_NguoiDungId",
                table: "DanhGiaChuyenGias",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoSucKhoes_NguoiDungId",
                table: "HoSoSucKhoes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachDinhDuongs_NguoiDungId",
                table: "KeHoachDinhDuongs",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachTapLuyens_NguoiDungId",
                table: "KeHoachTapLuyens",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_ChuyenGiaId",
                table: "LichHens",
                column: "ChuyenGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_NguoiDungId",
                table: "LichHens",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuSucKhoes_NguoiDungId",
                table: "LichSuSucKhoes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_VaiTroId",
                table: "NguoiDungs",
                column: "VaiTroId");

            migrationBuilder.CreateIndex(
                name: "IX_NhacNhoSucKhoe_NguoiDungId",
                table: "NhacNhoSucKhoe",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiSucKhoes_NguoiDungId",
                table: "PhanHoiSucKhoes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoBacSis_BacSiId",
                table: "ThongBaoBacSis",
                column: "BacSiId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoBacSis_NguoiDungId",
                table: "ThongBaoBacSis",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_TuVanSucKhoes_ChuyenGiaId",
                table: "TuVanSucKhoes",
                column: "ChuyenGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_TuVanSucKhoes_NguoiDungId",
                table: "TuVanSucKhoes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiSoSucKhoe");

            migrationBuilder.DropTable(
                name: "ChiTietKeHoachDinhDuongs");

            migrationBuilder.DropTable(
                name: "ChiTietKeHoachTapLuyens");

            migrationBuilder.DropTable(
                name: "DanhGiaChuyenGias");

            migrationBuilder.DropTable(
                name: "HoSoSucKhoes");

            migrationBuilder.DropTable(
                name: "LichHens");

            migrationBuilder.DropTable(
                name: "LichSuSucKhoes");

            migrationBuilder.DropTable(
                name: "NhacNhoSucKhoe");

            migrationBuilder.DropTable(
                name: "PhanHoiSucKhoes");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "ThongBaoBacSis");

            migrationBuilder.DropTable(
                name: "TuVanSucKhoes");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "KeHoachDinhDuongs");

            migrationBuilder.DropTable(
                name: "KeHoachTapLuyens");

            migrationBuilder.DropTable(
                name: "ChuyenGia");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "VaiTros");
        }
    }
}
