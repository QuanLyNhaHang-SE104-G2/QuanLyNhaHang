using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang.Extensions;

public static class DbExceptionExtensions
{
    /// <summary>
    /// Determines whether a <see cref="DbUpdateException"/> represents a
    /// primary-key (or unique) constraint violation for SQLite and MSSQL Server providers
    /// </summary>
    public static bool IsPrimaryKeyViolation(this DbUpdateException ex)
    {
        // SQLite: Evaluate extended error codes to avoid catching NOT NULL, FOREIGN KEY, or CHECK failures
        if (ex.InnerException is SqliteException sqliteEx)
        {
            // SQLITE_CONSTRAINT_PRIMARYKEY = 1555
            // SQLITE_CONSTRAINT_UNIQUE = 2067
            // See https://www.sqlite.org/rescode.html
            return sqliteEx.SqliteExtendedErrorCode == 1555 ||
                   sqliteEx.SqliteExtendedErrorCode == 2067;
        }

        // primary key constraint = 2627
        // unique index = 2601
        if (ex.InnerException is System.Data.Common.DbException dbEx)
        {
            int? number = dbEx.GetType().GetProperty("Number")?.GetValue(dbEx) as int?;
            return number == 2627 || number == 2601;
        }

        return false;
    }
}
