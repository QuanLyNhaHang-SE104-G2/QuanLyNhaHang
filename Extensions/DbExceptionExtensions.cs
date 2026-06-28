using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang.Extensions;

public static class DbExceptionExtensions
{
    /// <summary>
    /// Determines whether a <see cref="DbUpdateException"/> represents a
    /// primary-key (or unique) constraint violation, which indicates a
    /// TOCTOU race on ID generation and warrants a retry with a fresh ID.
    /// Works for both SQLite and SQL Server providers.
    /// </summary>
    public static bool IsPrimaryKeyViolation(this DbUpdateException ex)
    {
        // SQLite: SqliteException with ErrorCode 19 (SQLITE_CONSTRAINT)
        if (ex.InnerException is SqliteException sqliteEx)
        {
            return sqliteEx.SqliteErrorCode == 19;
        }

        // SQL Server: SqlException with number 2627 (constraint violation)
        // or 2601 (unique index violation)
        if (ex.InnerException is System.Data.Common.DbException dbEx)
        {
            int? number = dbEx.GetType().GetProperty("Number")?.GetValue(dbEx) as int?;
            return number == 2627 || number == 2601;
        }

        return false;
    }
}
