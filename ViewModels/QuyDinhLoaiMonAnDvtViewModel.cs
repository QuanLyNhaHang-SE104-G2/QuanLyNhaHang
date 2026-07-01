using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhLoaiMonAnDvtViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<string> _columnHeaders = [];

    [ObservableProperty]
    private ObservableCollection<LoaiMonAnDvtRowItemViewModel> _rows = [];

    public QuyDinhLoaiMonAnDvtViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo liên kết loại món ăn - đơn vị tính: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public async Task InitializeFormAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var loaiMonAns = await context.LoaiMonAn.AsNoTracking().OrderBy(lma => lma.TenLoaiMonAn).ToListAsync();
        var donViTinhs = await context.DonViTinh.AsNoTracking().OrderBy(dvt => dvt.TenDonViTinh).ToListAsync();
        var junctions = await context.LoaiMonAnDonViTinh.AsNoTracking().ToListAsync();

        var junctionSet = junctions.Select(j => (j.MaLoaiMonAn, j.MaDonViTinh)).ToHashSet();

        var headers = new ObservableCollection<string> { "Loại món ăn" };
        foreach (var dvt in donViTinhs)
        {
            headers.Add(dvt.TenDonViTinh);
        }
        ColumnHeaders = headers;

        var rows = new ObservableCollection<LoaiMonAnDvtRowItemViewModel>();
        foreach (var lma in loaiMonAns)
        {
            var cells = new ObservableCollection<LoaiMonAnDvtCellItemViewModel>();
            foreach (var dvt in donViTinhs)
            {
                cells.Add(new LoaiMonAnDvtCellItemViewModel
                {
                    MaDonViTinh = dvt.MaDonViTinh,
                    TenDonViTinh = dvt.TenDonViTinh,
                    IsChecked = junctionSet.Contains((lma.MaLoaiMonAn, dvt.MaDonViTinh))
                });
            }
            rows.Add(new LoaiMonAnDvtRowItemViewModel
            {
                MaLoaiMonAn = lma.MaLoaiMonAn,
                TenLoaiMonAn = lma.TenLoaiMonAn,
                Cells = cells
            });
        }
        Rows = rows;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var existingJunctions = await context.LoaiMonAnDonViTinh.ToListAsync();

            var checkedPairs = new HashSet<(string, string)>();
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.IsChecked)
                    {
                        checkedPairs.Add((row.MaLoaiMonAn, cell.MaDonViTinh));
                    }
                }
            }

            var toDelete = existingJunctions
                .Where(j => !checkedPairs.Contains((j.MaLoaiMonAn, j.MaDonViTinh)))
                .ToList();

            var existingSet = existingJunctions
                .Select(j => (j.MaLoaiMonAn, j.MaDonViTinh))
                .ToHashSet();

            var toInsert = checkedPairs
                .Where(p => !existingSet.Contains(p))
                .Select(p => new LoaiMonAnDonViTinh
                {
                    MaLoaiMonAn = p.Item1,
                    MaDonViTinh = p.Item2
                })
                .ToList();

            if (toDelete.Count > 0)
            {
                context.LoaiMonAnDonViTinh.RemoveRange(toDelete);
            }

            if (toInsert.Count > 0)
            {
                context.LoaiMonAnDonViTinh.AddRange(toInsert);
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi liên kết loại món ăn - đơn vị tính thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
