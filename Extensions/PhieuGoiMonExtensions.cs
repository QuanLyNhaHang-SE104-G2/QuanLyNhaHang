using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Extensions;

public static class PhieuGoiMonExtensions
{
    public static IQueryable<PhieuGoiMon> GetWithIncludes(this IQueryable<PhieuGoiMon> query)
    {
        return query
            .Include(p => p.Ban)
            .Include(p => p.NhanVien)
            .Include(p => p.TrangThai)
            .Include(p => p.CTGoiMons)
                .ThenInclude(c => c.MonAn)
                    .ThenInclude(m => m.DonViTinh);
    }

    public static IQueryable<PhieuGoiMon> Filter(
        this IQueryable<PhieuGoiMon> query,
        int? maPhieuGoiMon,
        string? tenBan,
        string? tenNhanVien,
        string? maTrangThai,
        long? minTotal,
        long? maxTotal)
    {
        if (maPhieuGoiMon.HasValue)
        {
            query = query.Where(p => p.MaPhieuGoiMon == maPhieuGoiMon.Value);
        }

        if (!string.IsNullOrWhiteSpace(tenBan))
        {
            query = query.Where(p => p.Ban.TenBan.Contains(tenBan));
        }

        if (!string.IsNullOrWhiteSpace(tenNhanVien))
        {
            query = query.Where(p => p.NhanVien.TenNhanVien.Contains(tenNhanVien));
        }

        if (!string.IsNullOrWhiteSpace(maTrangThai) && maTrangThai != "All")
        {
            query = query.Where(p => p.MaTrangThai == maTrangThai);
        }

        if (minTotal.HasValue)
        {
            query = query.Where(p => p.TongTienTamTinh >= minTotal.Value);
        }

        if (maxTotal.HasValue)
        {
            query = query.Where(p => p.TongTienTamTinh <= maxTotal.Value);
        }

        return query;
    }
}
