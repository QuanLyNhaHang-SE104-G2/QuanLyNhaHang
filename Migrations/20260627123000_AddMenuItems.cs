using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DONVITINH",
                columns: table => new
                {
                    MaDonViTinh = table.Column<string>(type: "TEXT", nullable: false),
                    TenDonViTinh = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONVITINH", x => x.MaDonViTinh);
                });

            migrationBuilder.CreateTable(
                name: "LOAIMONAN",
                columns: table => new
                {
                    MaLoaiMonAn = table.Column<string>(type: "TEXT", nullable: false),
                    TenLoaiMonAn = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAIMONAN", x => x.MaLoaiMonAn);
                });

            migrationBuilder.CreateTable(
                name: "TINHTRANG",
                columns: table => new
                {
                    MaTinhTrang = table.Column<string>(type: "TEXT", nullable: false),
                    TenTinhTrang = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TINHTRANG", x => x.MaTinhTrang);
                });

            migrationBuilder.CreateTable(
                name: "QD_LOAIMON_DVT",
                columns: table => new
                {
                    MaLoaiMonAn = table.Column<string>(type: "TEXT", nullable: false),
                    MaDonViTinh = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QD_LOAIMON_DVT", x => new { x.MaLoaiMonAn, x.MaDonViTinh });
                    table.ForeignKey(
                        name: "FK_QD_LOAIMON_DVT_DONVITINH_MaDonViTinh",
                        column: x => x.MaDonViTinh,
                        principalTable: "DONVITINH",
                        principalColumn: "MaDonViTinh",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QD_LOAIMON_DVT_LOAIMONAN_MaLoaiMonAn",
                        column: x => x.MaLoaiMonAn,
                        principalTable: "LOAIMONAN",
                        principalColumn: "MaLoaiMonAn",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MONAN",
                columns: table => new
                {
                    MaMonAn = table.Column<int>(type: "INTEGER", nullable: false),
                    TenMonAn = table.Column<string>(type: "TEXT", nullable: false),
                    DonGia = table.Column<long>(type: "INTEGER", nullable: false),
                    MaLoaiMonAn = table.Column<string>(type: "TEXT", nullable: false),
                    MaDonViTinh = table.Column<string>(type: "TEXT", nullable: false),
                    MaTinhTrang = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MONAN", x => x.MaMonAn);
                    table.ForeignKey(
                        name: "FK_MONAN_DONVITINH_MaDonViTinh",
                        column: x => x.MaDonViTinh,
                        principalTable: "DONVITINH",
                        principalColumn: "MaDonViTinh",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MONAN_LOAIMONAN_MaLoaiMonAn",
                        column: x => x.MaLoaiMonAn,
                        principalTable: "LOAIMONAN",
                        principalColumn: "MaLoaiMonAn",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MONAN_TINHTRANG_MaTinhTrang",
                        column: x => x.MaTinhTrang,
                        principalTable: "TINHTRANG",
                        principalColumn: "MaTinhTrang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DONVITINH",
                columns: new[] { "MaDonViTinh", "TenDonViTinh" },
                values: new object[,]
                {
                    { "Chai", "Chai" },
                    { "Dia", "Đĩa" },
                    { "Ly", "Ly" },
                    { "Phan", "Phần" }
                });

            migrationBuilder.InsertData(
                table: "LOAIMONAN",
                columns: new[] { "MaLoaiMonAn", "TenLoaiMonAn" },
                values: new object[,]
                {
                    { "Chinh", "Món chính" },
                    { "DoUong", "Đồ uống" },
                    { "KhaiVi", "Món khai vị" },
                    { "TrangMieng", "Tráng miệng" }
                });

            migrationBuilder.InsertData(
                table: "TINHTRANG",
                columns: new[] { "MaTinhTrang", "TenTinhTrang" },
                values: new object[,]
                {
                    { "DangBan", "Đang bán" },
                    { "NgungBan", "Ngừng bán" }
                });

            migrationBuilder.InsertData(
                table: "QD_LOAIMON_DVT",
                columns: new[] { "MaDonViTinh", "MaLoaiMonAn" },
                values: new object[,]
                {
                    { "Dia", "Chinh" },
                    { "Phan", "Chinh" },
                    { "Chai", "DoUong" },
                    { "Ly", "DoUong" },
                    { "Dia", "KhaiVi" },
                    { "Phan", "KhaiVi" },
                    { "Dia", "TrangMieng" },
                    { "Phan", "TrangMieng" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MONAN_MaDonViTinh",
                table: "MONAN",
                column: "MaDonViTinh");

            migrationBuilder.CreateIndex(
                name: "IX_MONAN_MaLoaiMonAn",
                table: "MONAN",
                column: "MaLoaiMonAn");

            migrationBuilder.CreateIndex(
                name: "IX_MONAN_MaTinhTrang",
                table: "MONAN",
                column: "MaTinhTrang");

            migrationBuilder.CreateIndex(
                name: "IX_QD_LOAIMON_DVT_MaDonViTinh",
                table: "QD_LOAIMON_DVT",
                column: "MaDonViTinh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MONAN");

            migrationBuilder.DropTable(
                name: "QD_LOAIMON_DVT");

            migrationBuilder.DropTable(
                name: "TINHTRANG");

            migrationBuilder.DropTable(
                name: "DONVITINH");

            migrationBuilder.DropTable(
                name: "LOAIMONAN");
        }
    }
}
