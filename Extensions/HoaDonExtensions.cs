using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Extensions;

public static class HoaDonExtensions
{
    public static IQueryable<HoaDon> GetWithIncludes(this IQueryable<HoaDon> query)
    {
        return query
            .Include(h => h.PhieuGoiMons)
                .ThenInclude(p => p.Ban);
    }

    public static IQueryable<HoaDon> Filter(
        this IQueryable<HoaDon> query,
        int? maHoaDon,
        string? tenBan,
        long? minTotal,
        long? maxTotal)
    {
        if (maHoaDon.HasValue)
        {
            query = query.Where(h => h.MaHoaDon == maHoaDon.Value);
        }

        if (!string.IsNullOrWhiteSpace(tenBan))
        {
            query = query.Where(h => h.PhieuGoiMons.Any(p => p.Ban.TenBan.Contains(tenBan)));
        }

        if (minTotal.HasValue)
        {
            query = query.Where(h => h.TongTien >= minTotal.Value);
        }

        if (maxTotal.HasValue)
        {
            query = query.Where(h => h.TongTien <= maxTotal.Value);
        }

        return query;
    }
}
