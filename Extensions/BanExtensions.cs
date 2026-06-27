using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Extensions;

public static class BanExtensions
{
    public static IQueryable<Ban> GetWithIncludes(this IQueryable<Ban> query)
    {
        return query.Include(b => b.LoaiBan);
    }

    public static IQueryable<Ban> Filter(
        this IQueryable<Ban> query,
        string? maBan,
        string? tenBan,
        string? khuVuc,
        string? maLoaiBan,
        int? minSeats,
        int? maxSeats,
        decimal? minPhuThu,
        decimal? maxPhuThu)
    {
        if (!string.IsNullOrWhiteSpace(maBan))
        {
            query = query.Where(b => b.MaBan.Contains(maBan));
        }

        if (!string.IsNullOrWhiteSpace(tenBan))
        {
            query = query.Where(b => b.TenBan.Contains(tenBan));
        }

        if (!string.IsNullOrWhiteSpace(khuVuc))
        {
            query = query.Where(b => b.KhuVuc.Contains(khuVuc));
        }

        if (!string.IsNullOrWhiteSpace(maLoaiBan) && maLoaiBan != "All")
        {
            query = query.Where(b => b.MaLoaiBan == maLoaiBan);
        }

        if (minSeats.HasValue)
        {
            query = query.Where(b => b.SoChoNgoi >= minSeats.Value);
        }

        if (maxSeats.HasValue)
        {
            query = query.Where(b => b.SoChoNgoi <= maxSeats.Value);
        }

        if (minPhuThu.HasValue)
        {
            query = query.Where(b => b.LoaiBan.PhuThu >= minPhuThu.Value);
        }

        if (maxPhuThu.HasValue)
        {
            query = query.Where(b => b.LoaiBan.PhuThu <= maxPhuThu.Value);
        }

        return query;
    }
}
