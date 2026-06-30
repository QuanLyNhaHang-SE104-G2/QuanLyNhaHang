using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class TrangThaiConstraintItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _maTrangThai = "";

    [ObservableProperty]
    private string _tenTrangThai = "";

    [ObservableProperty]
    private bool _duocThanhToan;
}

public partial class QuyDinh5ViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<TrangThaiConstraintItemViewModel> _statuses = [];

    public QuyDinh5ViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var statusesDb = await context.TrangThai.AsNoTracking().ToListAsync(cancellationToken);
        
        var vms = statusesDb.Select(s => new TrangThaiConstraintItemViewModel
        {
            MaTrangThai = s.MaTrangThai,
            TenTrangThai = s.TenTrangThai,
            DuocThanhToan = s.DuocThanhToan
        }).ToList();
        
        App.Current.Dispatcher.Invoke(() =>
        {
            Statuses = new ObservableCollection<TrangThaiConstraintItemViewModel>(vms);
        });
    }

    [RelayCommand]
    private async Task LuuThayDoiAsync()
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
                    context.TrangThai.Update(existing);
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi Quy định 5 thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lưu thay đổi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
