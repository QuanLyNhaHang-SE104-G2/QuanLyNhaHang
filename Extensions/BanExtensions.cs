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
}
