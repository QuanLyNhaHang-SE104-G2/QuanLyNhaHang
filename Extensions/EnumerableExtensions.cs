using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyNhaHang.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> GetPage<T>(
        this IEnumerable<T> source,
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
        return source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}
