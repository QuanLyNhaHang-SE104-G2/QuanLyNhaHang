using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> GetPage<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than or equal to 1.", nameof(pageNumber));
        }
        if (pageSize < 1)
        {
            throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));
        }
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}
