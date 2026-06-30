using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhTrangThaiThanhToanViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<TrangThaiConstraintItemViewModel> _statuses = [];

    public QuyDinhTrangThaiThanhToanViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo quy định thanh toán: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public async Task InitializeFormAsync()
    {
        Statuses.Clear();

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var statusesDb = await context.TrangThai.AsNoTracking().ToListAsync();
            var vms = statusesDb.Select(s => new TrangThaiConstraintItemViewModel
            {
                MaTrangThai = s.MaTrangThai,
                TenTrangThai = s.TenTrangThai,
                DuocThanhToan = s.DuocThanhToan
            }).ToList();

            Statuses = new ObservableCollection<TrangThaiConstraintItemViewModel>(vms);
        }
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var statusesDb = await context.TrangThai.ToListAsync();

            foreach (var vm in Statuses)
            {
                var existing = statusesDb.FirstOrDefault(x => x.MaTrangThai == vm.MaTrangThai);
                if (existing != null)
                {
                    existing.DuocThanhToan = vm.DuocThanhToan;
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi quy định thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
