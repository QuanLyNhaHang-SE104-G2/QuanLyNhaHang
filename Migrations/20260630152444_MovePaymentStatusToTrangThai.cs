using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class MovePaymentStatusToTrangThai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiThanhToan",
                table: "THAMSO");

            migrationBuilder.AddColumn<bool>(
                name: "DuocThanhToan",
                table: "TRANGTHAI",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TRANGTHAI",
                keyColumn: "MaTrangThai",
                keyValue: "ChoBep",
                column: "DuocThanhToan",
                value: false);

            migrationBuilder.UpdateData(
                table: "TRANGTHAI",
                keyColumn: "MaTrangThai",
                keyValue: "DangCheBien",
                column: "DuocThanhToan",
                value: true);

            migrationBuilder.UpdateData(
                table: "TRANGTHAI",
                keyColumn: "MaTrangThai",
                keyValue: "DaPhucVu",
                column: "DuocThanhToan",
                value: true);

            migrationBuilder.UpdateData(
                table: "TRANGTHAI",
                keyColumn: "MaTrangThai",
                keyValue: "Huy",
                column: "DuocThanhToan",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuocThanhToan",
                table: "TRANGTHAI");

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiThanhToan",
                table: "THAMSO",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "THAMSO",
                keyColumn: "Id",
                keyValue: 1,
                column: "TrangThaiThanhToan",
                value: "DangCheBien,DaPhucVu");
        }
    }
}
