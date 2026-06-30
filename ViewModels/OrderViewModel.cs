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

public partial class OrderViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "đơn";

    [ObservableProperty]
    private ObservableCollection<OrderItemViewModel> _orders = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteOrderCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditOrderCommand))]
    private OrderItemViewModel? _selectedOrder;

    private bool CanDeleteOrEdit => SelectedOrder != null;

    public OrderViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<PhieuGoiMon> items, int total)> QueryOrdersAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.PhieuGoiMon
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes();

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(p => p.MaPhieuGoiMon)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (rawOrders, totalItems) = await QueryOrdersAsync();
        TotalItems = totalItems;

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<OrderItemViewModel>(rawOrders.Count);
        foreach (var order in rawOrders)
        {
            page.Add(new OrderItemViewModel
            {
                STT = stt++,
                MaPhieuGoiMon = order.MaPhieuGoiMon,
                TenBan = order.Ban?.TenBan ?? "",
                ThoiGianGoiText = order.ThoiGianGoi.ToString("yyyy-MM-dd HH:mm"),
                TenNhanVien = order.NhanVien?.TenNhanVien ?? "",
                TenTrangThai = order.TrangThai?.TenTrangThai ?? "",
                MaTrangThai = order.MaTrangThai,
                TongTienText = $"{order.TongTienTamTinh:N0} VND"
            });
        }

        Orders = new ObservableCollection<OrderItemViewModel>(page);
    }

    [RelayCommand]
    private async Task AddOrderAsync(Window? owner)
    {
        if (_dialogService.ShowTiepNhanPhieuGoiMonDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private void SearchOrder(Window? owner)
    {
        _dialogService.ShowTraCuuPhieuGoiMonDialog(owner);
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task DeleteOrderAsync()
    {
        if (SelectedOrder == null) return;

        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu gọi món '{SelectedOrder.MaPhieuGoiMon}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var order = await context.PhieuGoiMon.FirstOrDefaultAsync(p => p.MaPhieuGoiMon == SelectedOrder.MaPhieuGoiMon);
                if (order != null)
                {
                    if (order.MaHoaDon.HasValue)
                    {
                        MessageBox.Show($"Không thể xóa phiếu gọi món '{SelectedOrder.MaPhieuGoiMon}' vì phiếu đã được thanh toán (đã xuất hóa đơn).", "Lỗi xóa phiếu gọi món", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var details = context.CTGoiMon.Where(c => c.MaPhieuGoiMon == order.MaPhieuGoiMon);
                    context.CTGoiMon.RemoveRange(details);

                    context.PhieuGoiMon.Remove(order);
                    await context.SaveChangesAsync();
                }
            }

            MessageBox.Show("Xóa phiếu gọi món thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedOrder = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task EditOrderAsync(Window? owner)
    {
        if (SelectedOrder == null) return;

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var order = await context.PhieuGoiMon.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhieuGoiMon == SelectedOrder.MaPhieuGoiMon);
            if (order != null && order.MaHoaDon.HasValue)
            {
                MessageBox.Show($"Không thể sửa phiếu gọi món '{SelectedOrder.MaPhieuGoiMon}' vì phiếu đã được thanh toán (đã xuất hóa đơn).", "Lỗi sửa phiếu gọi món", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        if (_dialogService.ShowCapNhatPhieuGoiMonDialog(owner, SelectedOrder.MaPhieuGoiMon) == true)
        {
            await LoadDataAsync();
        }
    }
}
