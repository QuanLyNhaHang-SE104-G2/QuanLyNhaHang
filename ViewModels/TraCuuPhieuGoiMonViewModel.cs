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

namespace QuanLyNhaHang.ViewModels;

public partial class TraCuuPhieuGoiMonViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private bool _hasSearched;

    protected override string EntityLabel => "đơn";

    [ObservableProperty]
    private string _maPhieuGoiMon = "";

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _tenNhanVien = "";

    [ObservableProperty]
    private string _selectedMaTrangThai = "All";

    [ObservableProperty]
    private string _tongTienTu = "";

    [ObservableProperty]
    private string _tongTienDen = "";

    [ObservableProperty]
    private List<TrangThaiOption> _trangThais = [];

    [ObservableProperty]
    private ObservableCollection<OrderItemViewModel> _orders = [];

    public record TrangThaiOption(string MaTrangThai, string TenTrangThai);

    public TraCuuPhieuGoiMonViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        PageSize = 10;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public async Task InitializeFormAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var rawStatuses = await context.TrangThai.AsNoTracking().OrderBy(t => t.TenTrangThai).ToListAsync();
        TrangThais = rawStatuses
            .Select(t => new TrangThaiOption(t.MaTrangThai, t.TenTrangThai))
            .Prepend(new TrangThaiOption("All", "Tất cả"))
            .ToList();

        SelectedMaTrangThai = "All";
        _hasSearched = false;
        await LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        if (!_hasSearched)
        {
            Orders = [];
            TotalItems = 0;
            UpdatePaginationInfo();
            return;
        }

        int? searchMaPhieuGoiMon = int.TryParse(MaPhieuGoiMon, out int parsedId) ? parsedId : null;

        long? minTotal = null;
        if (long.TryParse(TongTienTu, out long minVal)) minTotal = minVal;

        long? maxTotal = null;
        if (long.TryParse(TongTienDen, out long maxVal)) maxTotal = maxVal;

        cancellationToken.ThrowIfCancellationRequested();

        async Task<(List<PhieuGoiMon> items, int total)> QueryOrdersAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.PhieuGoiMon
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes()
                .Filter(searchMaPhieuGoiMon, TenBan, TenNhanVien, SelectedMaTrangThai, minTotal, maxTotal);

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(p => p.MaPhieuGoiMon)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

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
    private async Task SearchAsync()
    {
        _hasSearched = true;
        PageNumber = 1;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void Cancel(Window? window)
    {
        if (window != null)
        {
            window.Close();
        }
    }
}
