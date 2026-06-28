using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuanLyNhaHang.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        string connString = AppDbContext.LoadConnectionStringFromConfig() ?? "Data Source=QuanLyNhaHang.db";
        optionsBuilder.UseSqlite(connString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
