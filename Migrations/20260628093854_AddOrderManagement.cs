using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "TEXT", nullable: false),
                    TenNhanVien = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHANVIEN", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "TRANGTHAI",
                columns: table => new
                {
                    MaTrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    TenTrangThai = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANGTHAI", x => x.MaTrangThai);
                });

            migrationBuilder.CreateTable(
                name: "PHIEUGOIMON",
                columns: table => new
                {
                    MaPhieuGoiMon = table.Column<string>(type: "TEXT", nullable: false),
                    ThoiGianGoi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TongTienTamTinh = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaTrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    MaNhanVien = table.Column<string>(type: "TEXT", nullable: false),
                    MaBan = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHIEUGOIMON", x => x.MaPhieuGoiMon);
                    table.ForeignKey(
                        name: "FK_PHIEUGOIMON_BAN_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BAN",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PHIEUGOIMON_NHANVIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PHIEUGOIMON_TRANGTHAI_MaTrangThai",
                        column: x => x.MaTrangThai,
                        principalTable: "TRANGTHAI",
                        principalColumn: "MaTrangThai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTGOIMON",
                columns: table => new
                {
                    MaPhieuGoiMon = table.Column<string>(type: "TEXT", nullable: false),
                    MaMonAn = table.Column<string>(type: "TEXT", nullable: false),
                    SoLuong = table.Column<int>(type: "INTEGER", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: true),
                    DonGia = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTGOIMON", x => new { x.MaPhieuGoiMon, x.MaMonAn });
                    table.ForeignKey(
                        name: "FK_CTGOIMON_MONAN_MaMonAn",
                        column: x => x.MaMonAn,
                        principalTable: "MONAN",
                        principalColumn: "MaMonAn",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTGOIMON_PHIEUGOIMON_MaPhieuGoiMon",
                        column: x => x.MaPhieuGoiMon,
                        principalTable: "PHIEUGOIMON",
                        principalColumn: "MaPhieuGoiMon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NHANVIEN",
                columns: new[] { "MaNhanVien", "TenNhanVien" },
                values: new object[,]
                {
                    { "NV001", "Mai P." },
                    { "NV002", "Hoàng T." },
                    { "NV003", "Linh N." }
                });

            migrationBuilder.InsertData(
                table: "TRANGTHAI",
                columns: new[] { "MaTrangThai", "TenTrangThai" },
                values: new object[,]
                {
                    { "ChoBep", "Chờ bếp" },
                    { "DangCheBien", "Đang chế biến" },
                    { "DaPhucVu", "Đã phục vụ" },
                    { "Huy", "Huỷ" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CTGOIMON_MaMonAn",
                table: "CTGOIMON",
                column: "MaMonAn");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUGOIMON_MaBan",
                table: "PHIEUGOIMON",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUGOIMON_MaNhanVien",
                table: "PHIEUGOIMON",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUGOIMON_MaTrangThai",
                table: "PHIEUGOIMON",
                column: "MaTrangThai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTGOIMON");

            migrationBuilder.DropTable(
                name: "PHIEUGOIMON");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "TRANGTHAI");
        }
    }
}
