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

namespace QuanLyNhaHang.ViewModels;

public partial class BaoCaoDoanhThuTheoNgayViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "ngày";

    [ObservableProperty]
    private string _namBaoCao = "";

    [ObservableProperty]
    private string _thangBaoCao = "";

    [ObservableProperty]
    private long _tongDoanhThu;

    [ObservableProperty]
    private string _tongDoanhThuText = "0 VND";

    [ObservableProperty]
    private ObservableCollection<BaoCaoDoanhThuTheoNgayItemViewModel> _pagedItems = [];

    public BaoCaoDoanhThuTheoNgayViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        
        // Default to current year and month
        NamBaoCao = DateTime.Now.Year.ToString();
        ThangBaoCao = DateTime.Now.Month.ToString();
        
        PageSize = 10;
        
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        if (!int.TryParse(NamBaoCao, out int year) || year <= 0 ||
            !int.TryParse(ThangBaoCao, out int month) || month < 1 || month > 12)
        {
            PagedItems = [];
            TotalItems = 0;
            TongDoanhThu = 0;
            TongDoanhThuText = "0 VND";
            UpdatePaginationInfo();
            return;
        }

        async Task<(List<Models.HoaDon> invoices, long totalRevenue)> QueryMonthlyInvoicesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var invoices = await context.HoaDon
                .AsNoTracking()
                .Where(h => h.ThoiGianThanhToan.Year == year && h.ThoiGianThanhToan.Month == month)
                .ToListAsync(cancellationToken);

            long total = invoices.Sum(h => h.TongTien);
            return (invoices, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (invoices, totalRevenue) = await QueryMonthlyInvoicesAsync();
        TongDoanhThu = totalRevenue;
        TongDoanhThuText = $"{TongDoanhThu:N0} VND";

        // Group by day of month in memory
        var dailyTotals = invoices
            .GroupBy(h => h.ThoiGianThanhToan.Day)
            .Select(g => new
            {
                Day = g.Key,
                DoanhThu = g.Sum(h => h.TongTien)
            })
            .OrderBy(x => x.Day)
            .ToList();

        TotalItems = dailyTotals.Count;
        UpdatePaginationInfo();

        var sliced = dailyTotals
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<BaoCaoDoanhThuTheoNgayItemViewModel>(sliced.Count);
        foreach (var d in sliced)
        {
            double tyLe = TongDoanhThu == 0 ? 0.0 : (d.DoanhThu / (double)TongDoanhThu) * 100.0;
            string dateText = $"{d.Day:D2}/{month:D2}/{year}";

            page.Add(new BaoCaoDoanhThuTheoNgayItemViewModel
            {
                STT = stt++,
                NgayText = dateText,
                DoanhThu = d.DoanhThu,
                DoanhThuText = $"{d.DoanhThu:N0} VND",
                TyLe = tyLe,
                TyLeText = $"{tyLe:F2}%"
            });
        }

        PagedItems = new ObservableCollection<BaoCaoDoanhThuTheoNgayItemViewModel>(page);
    }

    [RelayCommand]
    private async Task LapBaoCaoAsync()
    {
        if (!int.TryParse(NamBaoCao, out int year) || year <= 0)
        {
            MessageBox.Show("Năm lập báo cáo phải là số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (!int.TryParse(ThangBaoCao, out int month) || month < 1 || month > 12)
        {
            MessageBox.Show("Tháng lập báo cáo phải từ 1 đến 12.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        PageNumber = 1;
        await LoadDataAsync();
    }
}
