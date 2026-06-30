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

public partial class BaoCaoDoanhThuTheoMonAnViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "món ăn";

    [ObservableProperty]
    private string _namBaoCao = "";

    [ObservableProperty]
    private string _thangBaoCao = "";

    [ObservableProperty]
    private string _selectedMaLoaiMonAn = "";

    [ObservableProperty]
    private List<LoaiMonAn> _categories = [];

    [ObservableProperty]
    private long _tongDoanhThu;

    [ObservableProperty]
    private string _tongDoanhThuText = "0 VND";

    [ObservableProperty]
    private ObservableCollection<BaoCaoDoanhThuTheoMonAnItemViewModel> _pagedItems = [];

    private class DishRevenueResult
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; } = "";
        public long DoanhThu { get; set; }
    }

    public BaoCaoDoanhThuTheoMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        NamBaoCao = DateTime.Now.Year.ToString();
        ThangBaoCao = DateTime.Now.Month.ToString();

        PageSize = 10;

        InitializeAsync().SafeFireAndForget();
    }

    private async Task InitializeAsync()
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            Categories = await context.LoaiMonAn.AsNoTracking().ToListAsync();
        }

        if (Categories.Count > 0)
        {
            SelectedMaLoaiMonAn = Categories[0].MaLoaiMonAn;
        }

        await LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(SelectedMaLoaiMonAn) ||
            !int.TryParse(NamBaoCao, out int year) || year <= 0 ||
            !int.TryParse(ThangBaoCao, out int month) || month < 1 || month > 12)
        {
            PagedItems = [];
            TotalItems = 0;
            TongDoanhThu = 0;
            TongDoanhThuText = "0 VND";
            UpdatePaginationInfo();
            return;
        }

        async Task<(List<DishRevenueResult> items, int total, long totalCategoryRevenue)> QueryDishRevenueAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.CTGoiMon
                .AsNoTracking()
                .Where(ct => ct.PhieuGoiMon.MaHoaDon != null &&
                             ct.PhieuGoiMon.HoaDon!.ThoiGianThanhToan.Year == year &&
                             ct.PhieuGoiMon.HoaDon!.ThoiGianThanhToan.Month == month &&
                             ct.MonAn.MaLoaiMonAn == SelectedMaLoaiMonAn);

            long totalCategoryRevenue = await query.SumAsync(ct => ct.SoLuong * ct.DonGia, cancellationToken);

            var groupedQuery = query
                .GroupBy(ct => new { ct.MaMonAn, ct.MonAn.TenMonAn })
                .Select(g => new DishRevenueResult
                {
                    MaMonAn = g.Key.MaMonAn,
                    TenMonAn = g.Key.TenMonAn,
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.DonGia)
                });

            int total = await groupedQuery.CountAsync(cancellationToken);
            var items = await groupedQuery
                .OrderByDescending(x => x.DoanhThu)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(cancellationToken);

            return (items, total, totalCategoryRevenue);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (rawItems, totalItems, totalCategoryRevenue) = await QueryDishRevenueAsync();
        TotalItems = totalItems;
        TongDoanhThu = totalCategoryRevenue;
        TongDoanhThuText = $"{TongDoanhThu:N0} VND";

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<BaoCaoDoanhThuTheoMonAnItemViewModel>(rawItems.Count);
        foreach (var item in rawItems)
        {
            double tyLe = TongDoanhThu == 0 ? 0.0 : (item.DoanhThu / (double)TongDoanhThu) * 100.0;

            page.Add(new BaoCaoDoanhThuTheoMonAnItemViewModel
            {
                STT = stt++,
                TenMonAn = item.TenMonAn,
                DoanhThu = item.DoanhThu,
                DoanhThuText = $"{item.DoanhThu:N0} VND",
                TyLe = tyLe,
                TyLeText = $"{tyLe:F2}%"
            });
        }

        PagedItems = new ObservableCollection<BaoCaoDoanhThuTheoMonAnItemViewModel>(page);
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

        if (string.IsNullOrEmpty(SelectedMaLoaiMonAn))
        {
            MessageBox.Show("Vui lòng chọn loại món ăn.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        PageNumber = 1;
        await LoadDataAsync();
    }
}
