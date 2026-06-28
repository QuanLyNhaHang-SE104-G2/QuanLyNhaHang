using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOAIBAN",
                columns: table => new
                {
                    MaLoaiBan = table.Column<string>(type: "TEXT", nullable: false),
                    TenLoaiBan = table.Column<string>(type: "TEXT", nullable: false),
                    PhuThu = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAIBAN", x => x.MaLoaiBan);
                });

            migrationBuilder.CreateTable(
                name: "THAMSO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SoChoNgoiToiThieu = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THAMSO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BAN",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "INTEGER", nullable: false),
                    TenBan = table.Column<string>(type: "TEXT", nullable: false),
                    KhuVuc = table.Column<string>(type: "TEXT", nullable: false),
                    SoChoNgoi = table.Column<int>(type: "INTEGER", nullable: false),
                    MaLoaiBan = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BAN", x => x.MaBan);
                    table.ForeignKey(
                        name: "FK_BAN_LOAIBAN_MaLoaiBan",
                        column: x => x.MaLoaiBan,
                        principalTable: "LOAIBAN",
                        principalColumn: "MaLoaiBan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LOAIBAN",
                columns: new[] { "MaLoaiBan", "PhuThu", "TenLoaiBan" },
                values: new object[,]
                {
                    { "Thuong", 0L, "Thường" },
                    { "VIP", 50000L, "VIP" },
                    { "VVIP", 80000L, "VVIP" }
                });

            migrationBuilder.InsertData(
                table: "THAMSO",
                columns: new[] { "Id", "SoChoNgoiToiThieu" },
                values: new object[] { 1, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_BAN_MaLoaiBan",
                table: "BAN",
                column: "MaLoaiBan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BAN");

            migrationBuilder.DropTable(
                name: "THAMSO");

            migrationBuilder.DropTable(
                name: "LOAIBAN");
        }
    }
}
