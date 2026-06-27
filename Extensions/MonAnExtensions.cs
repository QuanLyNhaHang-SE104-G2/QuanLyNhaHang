using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Extensions;

public static class MonAnExtensions
{
    public static IQueryable<MonAn> GetMonAnWithIncludes(this IQueryable<MonAn> query)
    {
        return query
            .Include(m => m.LoaiMonAn)
            .Include(m => m.DonViTinh)
            .Include(m => m.TinhTrang);
    }

    public static IQueryable<MonAn> SearchByName(this IQueryable<MonAn> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return query;

        return query.Where(m => m.TenMonAn.Contains(searchText));
    }

    public static IQueryable<MonAn> FilterByLoaiMonAn(this IQueryable<MonAn> query, string? maLoaiMonAn)
    {
        if (string.IsNullOrWhiteSpace(maLoaiMonAn))
            return query;

        return query.Where(m => m.MaLoaiMonAn == maLoaiMonAn);
    }

    public static IQueryable<DonViTinh> GetAllowedDonViTinh(this IQueryable<LoaiMonAnDonViTinh> query, string maLoaiMonAn)
    {
        return query
            .Where(q => q.MaLoaiMonAn == maLoaiMonAn)
            .Select(q => q.DonViTinh);
    }
}
