using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaHoaDon",
                table: "PHIEUGOIMON",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HOADON",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "INTEGER", nullable: false),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TongTien = table.Column<long>(type: "INTEGER", nullable: false),
                    PhuThu = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOADON", x => x.MaHoaDon);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUGOIMON_MaHoaDon",
                table: "PHIEUGOIMON",
                column: "MaHoaDon");

            migrationBuilder.AddForeignKey(
                name: "FK_PHIEUGOIMON_HOADON_MaHoaDon",
                table: "PHIEUGOIMON",
                column: "MaHoaDon",
                principalTable: "HOADON",
                principalColumn: "MaHoaDon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PHIEUGOIMON_HOADON_MaHoaDon",
                table: "PHIEUGOIMON");

            migrationBuilder.DropTable(
                name: "HOADON");

            migrationBuilder.DropIndex(
                name: "IX_PHIEUGOIMON_MaHoaDon",
                table: "PHIEUGOIMON");

            migrationBuilder.DropColumn(
                name: "MaHoaDon",
                table: "PHIEUGOIMON");
        }
    }
}
