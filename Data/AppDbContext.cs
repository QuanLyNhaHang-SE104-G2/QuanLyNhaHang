using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Data;

public class AppDbContext: DbContext
{
    public DbSet<Ban> Ban { get; set; } = null!;
    public DbSet<LoaiBan> LoaiBan { get; set; } = null!;
    public DbSet<ThamSo> ThamSo { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlite("Data Source=QuanLyNhaHang.db");
    }
}
