using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.ViewModels;

public partial class NhanVienViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "nhân viên";

    [ObservableProperty]
    private ObservableCollection<NhanVienItemViewModel> _nhanViens = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteNhanVienCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditNhanVienCommand))]
    private NhanVienItemViewModel? _selectedNhanVien;

    private bool CanDeleteOrEdit => SelectedNhanVien != null;

    public NhanVienViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<NhanVien> items, int total)> QueryNhanViensAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            int total = await context.NhanVien.CountAsync(cancellationToken);
            var items = await context.NhanVien
                .AsNoTracking()
                .OrderBy(n => n.MaNhanVien)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (rawItems, totalItems) = await QueryNhanViensAsync();
        TotalItems = totalItems;

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<NhanVienItemViewModel>(rawItems.Count);
        foreach (var item in rawItems)
        {
            page.Add(new NhanVienItemViewModel
            {
                STT = stt++,
                MaNhanVien = item.MaNhanVien,
                TenNhanVien = item.TenNhanVien
            });
        }

        NhanViens = new ObservableCollection<NhanVienItemViewModel>(page);
    }

    [RelayCommand]
    private async Task AddNhanVienAsync(Window? owner)
    {
        if (_dialogService.ShowTiepNhanNhanVienDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task EditNhanVienAsync(Window? owner)
    {
        if (SelectedNhanVien == null) return;

        if (_dialogService.ShowCapNhatNhanVienDialog(owner, SelectedNhanVien.MaNhanVien) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task DeleteNhanVienAsync()
    {
        if (SelectedNhanVien == null) return;

        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên '{SelectedNhanVien.TenNhanVien}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                bool hasOrders = await context.PhieuGoiMon.AsNoTracking().AnyAsync(p => p.MaNhanVien == SelectedNhanVien.MaNhanVien);
                if (hasOrders)
                {
                    MessageBox.Show($"Không thể xóa nhân viên '{SelectedNhanVien.TenNhanVien}' vì nhân viên đang có phiếu gọi món liên kết.", "Lỗi xóa nhân viên", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var nv = await context.NhanVien.FirstOrDefaultAsync(n => n.MaNhanVien == SelectedNhanVien.MaNhanVien);
                if (nv != null)
                {
                    context.NhanVien.Remove(nv);
                    await context.SaveChangesAsync();
                }
            }

            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedNhanVien = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
