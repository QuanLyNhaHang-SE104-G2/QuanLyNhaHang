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
                ThoiGianGoiText = $"{order.ThoiGianGoi.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)} Hôm nay",
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
}
