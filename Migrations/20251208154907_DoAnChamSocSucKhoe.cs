using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnChamSocSucKhoe.Migrations
{
    /// <inheritdoc />
    public partial class DoAnChamSocSucKhoe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseInsurance = table.Column<bool>(type: "bit", nullable: false),
                    WantReminder = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhacNhoSucKhoes",
                columns: table => new
                {
                    NhacNhoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaThucHien = table.Column<bool>(type: "bit", nullable: false),
                    LoaiNhacNho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhacNhoSucKhoes", x => x.NhacNhoSucKhoeId);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    VaiTroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.VaiTroId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
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
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaiTroId = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_VaiTros_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "VaiTros",
                        principalColumn: "VaiTroId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
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
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoSucKhoes",
                columns: table => new
                {
                    ChiSoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChieuCao = table.Column<float>(type: "real", nullable: false),
                    CanNang = table.Column<float>(type: "real", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    HuyetAp = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<float>(type: "real", nullable: false),
                    NgayDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiChiSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoSucKhoes", x => x.ChiSoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_ChiSoSucKhoes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenGias",
                columns: table => new
                {
                    ChuyenGiaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChungChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KinhNghiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiCongTac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenGias", x => x.ChuyenGiaId);
                    table.ForeignKey(
                        name: "FK_ChuyenGias_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoSucKhoes",
                columns: table => new
                {
                    HoSoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChieuCao = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CanNang = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HuyetApTamThu = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HuyetApTamTruong = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NhomMau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienSuBenh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiUng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThuocDangDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienSuGiaDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoiSong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhuongPhapDieuTri = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoSucKhoes", x => x.HoSoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_HoSoSucKhoes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachDinhDuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeHoachDinhDuongId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachDinhDuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeHoachDinhDuongs_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachTapLuyens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachTapLuyens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeHoachTapLuyens_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LichHenId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChuyenGiaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayGioHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiLichHen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonThuoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BacSiId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BenhNhanId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichHens_AspNetUsers_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichHens_AspNetUsers_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHens_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuSucKhoes",
                columns: table => new
                {
                    LichSuSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChieuCao = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CanNang = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NhipTim = table.Column<int>(type: "int", nullable: false),
                    DuongHuyet = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HuyetApTamThu = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HuyetApTamTruong = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NgayDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuSucKhoes", x => x.LichSuSucKhoeId);
                    table.ForeignKey(
                        name: "FK_LichSuSucKhoes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiChamSocBenhNhans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiChamSocId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BenhNhanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuanHe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiChamSocBenhNhans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NguoiChamSocBenhNhans_AspNetUsers_BenhNhanId",
                        column: x => x.BenhNhanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NguoiChamSocBenhNhans_AspNetUsers_NguoiChamSocId",
                        column: x => x.NguoiChamSocId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanHoiSucKhoes",
                columns: table => new
                {
                    PhanHoiSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoiSucKhoes", x => x.PhanHoiSucKhoeId);
                    table.ForeignKey(
                        name: "FK_PhanHoiSucKhoes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaoBacSis",
                columns: table => new
                {
                    ThongBaoBacSiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BacSiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoBacSis", x => x.ThongBaoBacSiId);
                    table.ForeignKey(
                        name: "FK_ThongBaoBacSis_AspNetUsers_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBaoBacSis_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TuVanSucKhoes",
                columns: table => new
                {
                    TuVanSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChuyenGiaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraLoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraLoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuVanSucKhoes", x => x.TuVanSucKhoeId);
                    table.ForeignKey(
                        name: "FK_TuVanSucKhoes_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TuVanSucKhoes_ChuyenGias_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "ChuyenGias",
                        principalColumn: "ChuyenGiaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileHoSos",
                columns: table => new
                {
                    FileHoSoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoSoSucKhoeId = table.Column<int>(type: "int", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KichThuoc = table.Column<long>(type: "bigint", nullable: false),
                    NgayTaiLen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTaiLenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileHoSos", x => x.FileHoSoId);
                    table.ForeignKey(
                        name: "FK_FileHoSos_AspNetUsers_NguoiTaiLenId",
                        column: x => x.NguoiTaiLenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileHoSos_HoSoSucKhoes_HoSoSucKhoeId",
                        column: x => x.HoSoSucKhoeId,
                        principalTable: "HoSoSucKhoes",
                        principalColumn: "HoSoSucKhoeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuHoSoSucKhoes",
                columns: table => new
                {
                    LichSuHoSoSucKhoeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoSoSucKhoeId = table.Column<int>(type: "int", nullable: false),
                    NguoiThayDoiId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgayThayDoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThayDoiNoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuHoSoSucKhoes", x => x.LichSuHoSoSucKhoeId);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoSucKhoes_AspNetUsers_NguoiThayDoiId",
                        column: x => x.NguoiThayDoiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichSuHoSoSucKhoes_HoSoSucKhoes_HoSoSucKhoeId",
                        column: x => x.HoSoSucKhoeId,
                        principalTable: "HoSoSucKhoes",
                        principalColumn: "HoSoSucKhoeId",
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
                    SoLuong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKeHoachTapLuyens",
                columns: table => new
                {
                    ChiTietKeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeHoachTapLuyenId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_ChiTietKeHoachTapLuyens_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietKeHoachTapLuyens_KeHoachTapLuyens_KeHoachTapLuyenId",
                        column: x => x.KeHoachTapLuyenId,
                        principalTable: "KeHoachTapLuyens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaChuyenGias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TuVanSucKhoeId = table.Column<int>(type: "int", nullable: true),
                    ChuyenGiaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SoSao = table.Column<int>(type: "int", nullable: true),
                    BinhLuan = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaChuyenGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhGiaChuyenGias_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGiaChuyenGias_ChuyenGias_ChuyenGiaId",
                        column: x => x.ChuyenGiaId,
                        principalTable: "ChuyenGias",
                        principalColumn: "ChuyenGiaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGiaChuyenGias_TuVanSucKhoes_TuVanSucKhoeId",
                        column: x => x.TuVanSucKhoeId,
                        principalTable: "TuVanSucKhoes",
                        principalColumn: "TuVanSucKhoeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TuVanSucKhoeId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_TuVanSucKhoes_TuVanSucKhoeId",
                        column: x => x.TuVanSucKhoeId,
                        principalTable: "TuVanSucKhoes",
                        principalColumn: "TuVanSucKhoeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VaiTroId",
                table: "AspNetUsers",
                column: "VaiTroId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoSucKhoes_NguoiDungId",
                table: "ChiSoSucKhoes",
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
                name: "IX_ChiTietKeHoachTapLuyens_NguoiDungId",
                table: "ChiTietKeHoachTapLuyens",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenGias_NguoiDungId",
                table: "ChuyenGias",
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
                name: "IX_DanhGiaChuyenGias_TuVanSucKhoeId",
                table: "DanhGiaChuyenGias",
                column: "TuVanSucKhoeId",
                unique: true,
                filter: "[TuVanSucKhoeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FileHoSos_HoSoSucKhoeId",
                table: "FileHoSos",
                column: "HoSoSucKhoeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileHoSos_NguoiTaiLenId",
                table: "FileHoSos",
                column: "NguoiTaiLenId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoSucKhoes_NguoiDungId",
                table: "HoSoSucKhoes",
                column: "NguoiDungId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachDinhDuongs_NguoiDungId",
                table: "KeHoachDinhDuongs",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachTapLuyens_NguoiDungId",
                table: "KeHoachTapLuyens",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_BacSiId",
                table: "LichHens",
                column: "BacSiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_ChuyenGiaId",
                table: "LichHens",
                column: "ChuyenGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_NguoiDungId",
                table: "LichHens",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoSucKhoes_HoSoSucKhoeId",
                table: "LichSuHoSoSucKhoes",
                column: "HoSoSucKhoeId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoSucKhoes_NguoiThayDoiId",
                table: "LichSuHoSoSucKhoes",
                column: "NguoiThayDoiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuSucKhoes_NguoiDungId",
                table: "LichSuSucKhoes",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TuVanSucKhoeId",
                table: "Messages",
                column: "TuVanSucKhoeId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiChamSocBenhNhans_BenhNhanId",
                table: "NguoiChamSocBenhNhans",
                column: "BenhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiChamSocBenhNhans_NguoiChamSocId",
                table: "NguoiChamSocBenhNhans",
                column: "NguoiChamSocId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiSucKhoes_NguoiDungId",
                table: "PhanHoiSucKhoes",
                column: "NguoiDungId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiSoSucKhoes");

            migrationBuilder.DropTable(
                name: "ChiTietKeHoachDinhDuongs");

            migrationBuilder.DropTable(
                name: "ChiTietKeHoachTapLuyens");

            migrationBuilder.DropTable(
                name: "DanhGiaChuyenGias");

            migrationBuilder.DropTable(
                name: "FileHoSos");

            migrationBuilder.DropTable(
                name: "LichHens");

            migrationBuilder.DropTable(
                name: "LichSuHoSoSucKhoes");

            migrationBuilder.DropTable(
                name: "LichSuSucKhoes");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "NguoiChamSocBenhNhans");

            migrationBuilder.DropTable(
                name: "NhacNhoSucKhoes");

            migrationBuilder.DropTable(
                name: "PhanHoiSucKhoes");

            migrationBuilder.DropTable(
                name: "ThongBaoBacSis");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "KeHoachDinhDuongs");

            migrationBuilder.DropTable(
                name: "KeHoachTapLuyens");

            migrationBuilder.DropTable(
                name: "HoSoSucKhoes");

            migrationBuilder.DropTable(
                name: "TuVanSucKhoes");

            migrationBuilder.DropTable(
                name: "ChuyenGias");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "VaiTros");
        }
    }
}
