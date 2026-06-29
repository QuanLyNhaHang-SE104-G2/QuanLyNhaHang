using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Data;

public class AppDbContext: DbContext
{
    public DbSet<Ban> Ban { get; set; } = null!;
    public DbSet<LoaiBan> LoaiBan { get; set; } = null!;
    public DbSet<ThamSo> ThamSo { get; set; } = null!;
    public DbSet<MonAn> MonAn { get; set; } = null!;
    public DbSet<LoaiMonAn> LoaiMonAn { get; set; } = null!;
    public DbSet<DonViTinh> DonViTinh { get; set; } = null!;
    public DbSet<TinhTrang> TinhTrang { get; set; } = null!;
    public DbSet<LoaiMonAnDonViTinh> LoaiMonAnDonViTinh { get; set; } = null!;
    public DbSet<TrangThai> TrangThai { get; set; } = null!;
    public DbSet<NhanVien> NhanVien { get; set; } = null!;
    public DbSet<PhieuGoiMon> PhieuGoiMon { get; set; } = null!;
    public DbSet<CTGoiMon> CTGoiMon { get; set; } = null!;
    public DbSet<HoaDon> HoaDon { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LoaiMonAnDonViTinh>()
            .HasKey(q => new { q.MaLoaiMonAn, q.MaDonViTinh });

        modelBuilder.Entity<CTGoiMon>()
            .HasKey(c => new { c.MaPhieuGoiMon, c.MaMonAn });

        modelBuilder.Entity<LoaiBan>().HasData(
            new LoaiBan { MaLoaiBan = "Thuong", TenLoaiBan = "Thường", PhuThu = 0 },
            new LoaiBan { MaLoaiBan = "VIP", TenLoaiBan = "VIP", PhuThu = 50000 },
            new LoaiBan { MaLoaiBan = "VVIP", TenLoaiBan = "VVIP", PhuThu = 80000 }
        );

        modelBuilder.Entity<ThamSo>().HasData(
            new ThamSo { Id = 1, SoChoNgoiToiThieu = 2 }
        );

        modelBuilder.Entity<LoaiMonAn>().HasData(
            new LoaiMonAn { MaLoaiMonAn = "KhaiVi", TenLoaiMonAn = "Món khai vị" },
            new LoaiMonAn { MaLoaiMonAn = "Chinh", TenLoaiMonAn = "Món chính" },
            new LoaiMonAn { MaLoaiMonAn = "TrangMieng", TenLoaiMonAn = "Tráng miệng" },
            new LoaiMonAn { MaLoaiMonAn = "DoUong", TenLoaiMonAn = "Đồ uống" }
        );

        modelBuilder.Entity<DonViTinh>().HasData(
            new DonViTinh { MaDonViTinh = "Dia", TenDonViTinh = "Đĩa" },
            new DonViTinh { MaDonViTinh = "Phan", TenDonViTinh = "Phần" },
            new DonViTinh { MaDonViTinh = "Chai", TenDonViTinh = "Chai" },
            new DonViTinh { MaDonViTinh = "Ly", TenDonViTinh = "Ly" }
        );

        modelBuilder.Entity<TinhTrang>().HasData(
            new TinhTrang { MaTinhTrang = "DangBan", TenTinhTrang = "Đang bán" },
            new TinhTrang { MaTinhTrang = "NgungBan", TenTinhTrang = "Ngừng bán" }
        );

        modelBuilder.Entity<LoaiMonAnDonViTinh>().HasData(
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "KhaiVi", MaDonViTinh = "Dia" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "KhaiVi", MaDonViTinh = "Phan" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "Chinh", MaDonViTinh = "Dia" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "Chinh", MaDonViTinh = "Phan" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "TrangMieng", MaDonViTinh = "Dia" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "TrangMieng", MaDonViTinh = "Phan" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "DoUong", MaDonViTinh = "Chai" },
            new LoaiMonAnDonViTinh { MaLoaiMonAn = "DoUong", MaDonViTinh = "Ly" }
        );

        modelBuilder.Entity<TrangThai>().HasData(
            new TrangThai { MaTrangThai = "ChoBep", TenTrangThai = "Chờ bếp" },
            new TrangThai { MaTrangThai = "DangCheBien", TenTrangThai = "Đang chế biến" },
            new TrangThai { MaTrangThai = "DaPhucVu", TenTrangThai = "Đã phục vụ" },
            new TrangThai { MaTrangThai = "Huy", TenTrangThai = "Huỷ" }
        );


    }

    public static string? LoadConnectionStringFromConfig()
    {
        try
        {
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (System.IO.File.Exists(configPath))
            {
                string json = System.IO.File.ReadAllText(configPath);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ConnectionStrings", out var connStrings) &&
                    connStrings.TryGetProperty("DefaultConnection", out var defaultConn))
                {
                    return defaultConn.GetString();
                }
            }
        }
        catch
        {
            // Ignore and fallback
        }
        return null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connString = LoadConnectionStringFromConfig() ?? "Data Source=QuanLyNhaHang.db";
            optionsBuilder
                .UseSqlite(connString);
        }
    }
}
