using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Extensions;

public static class MonAnExtensions
{
    public static IQueryable<MonAn> GetWithIncludes(this IQueryable<MonAn> query)
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

    public static IQueryable<MonAn> Filter(
        this IQueryable<MonAn> query,
        int? maMonAn,
        string? tenMonAn,
        string? maLoaiMonAn,
        string? maDonViTinh,
        string? maTinhTrang,
        long? minPrice,
        long? maxPrice)
    {
        if (maMonAn.HasValue)
        {
            query = query.Where(m => m.MaMonAn == maMonAn.Value);
        }
        if (!string.IsNullOrWhiteSpace(tenMonAn))
        {
            query = query.Where(m => m.TenMonAn.Contains(tenMonAn));
        }
        if (!string.IsNullOrWhiteSpace(maLoaiMonAn) && maLoaiMonAn != "All")
        {
            query = query.Where(m => m.MaLoaiMonAn == maLoaiMonAn);
        }
        if (!string.IsNullOrWhiteSpace(maDonViTinh) && maDonViTinh != "All")
        {
            query = query.Where(m => m.MaDonViTinh == maDonViTinh);
        }
        if (!string.IsNullOrWhiteSpace(maTinhTrang) && maTinhTrang != "All")
        {
            query = query.Where(m => m.MaTinhTrang == maTinhTrang);
        }
        if (minPrice.HasValue)
        {
            query = query.Where(m => m.DonGia >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(m => m.DonGia <= maxPrice.Value);
        }
        return query;
    }
}
