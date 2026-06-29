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

public partial class TraCuuHoaDonViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private bool _hasSearched;

    protected override string EntityLabel => "hóa đơn";

    [ObservableProperty]
    private string _maHoaDon = "";

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _tongTienTu = "";

    [ObservableProperty]
    private string _tongTienDen = "";

    [ObservableProperty]
    private ObservableCollection<HoaDonItemViewModel> _invoices = [];

    public TraCuuHoaDonViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        PageSize = 10;
        _hasSearched = false;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        if (!_hasSearched)
        {
            Invoices = [];
            TotalItems = 0;
            UpdatePaginationInfo();
            return;
        }

        int? searchMaHoaDon = int.TryParse(MaHoaDon, out int parsedId) ? parsedId : null;

        long? minTotal = null;
        if (long.TryParse(TongTienTu, out long minVal)) minTotal = minVal;

        long? maxTotal = null;
        if (long.TryParse(TongTienDen, out long maxVal)) maxTotal = maxVal;

        cancellationToken.ThrowIfCancellationRequested();

        async Task<(List<HoaDon> items, int total)> QueryInvoicesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.HoaDon
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes()
                .Filter(searchMaHoaDon, TenBan, minTotal, maxTotal);

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(h => h.MaHoaDon)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        var (rawInvoices, totalItems) = await QueryInvoicesAsync();
        TotalItems = totalItems;

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<HoaDonItemViewModel>(rawInvoices.Count);
        foreach (var invoice in rawInvoices)
        {
            page.Add(new HoaDonItemViewModel
            {
                STT = stt++,
                MaHoaDon = invoice.MaHoaDon,
                TenBan = invoice.PhieuGoiMons.FirstOrDefault()?.Ban?.TenBan ?? "",
                ThoiGianThanhToanText = invoice.ThoiGianThanhToan.ToString("yyyy-MM-dd HH:mm"),
                PhuThuText = $"{invoice.PhuThu:N0} VND",
                TongTienText = $"{invoice.TongTien:N0} VND"
            });
        }

        Invoices = new ObservableCollection<HoaDonItemViewModel>(page);
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
