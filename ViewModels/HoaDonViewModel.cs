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

public partial class HoaDonViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "hóa đơn";

    [ObservableProperty]
    private ObservableCollection<HoaDonItemViewModel> _invoices = [];

    public HoaDonViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<HoaDon> items, int total)> QueryInvoicesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.HoaDon
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes();

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(h => h.MaHoaDon)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

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
    private async Task AddHoaDonAsync(Window? owner)
    {
        if (_dialogService.ShowTiepNhanHoaDonDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private void SearchHoaDon(Window? owner)
    {
        _dialogService.ShowTraCuuHoaDonDialog(owner);
    }
}
