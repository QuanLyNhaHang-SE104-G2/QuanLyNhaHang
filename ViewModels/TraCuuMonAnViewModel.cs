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

public partial class TraCuuMonAnViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private bool _hasSearched;

    protected override string EntityLabel => "món";

    [ObservableProperty]
    private string _maMonAn = "";

    [ObservableProperty]
    private string _tenMonAn = "";

    [ObservableProperty]
    private string _selectedMaLoaiMonAn = "All";

    [ObservableProperty]
    private string _selectedMaDonViTinh = "All";

    [ObservableProperty]
    private string _selectedMaTinhTrang = "All";

    [ObservableProperty]
    private string _donGiaTu = "";

    [ObservableProperty]
    private string _donGiaDen = "";

    [ObservableProperty]
    private List<LoaiMonAnOption> _loaiMonAns = [];

    [ObservableProperty]
    private List<DonViTinhOption> _donViTinhs = [];

    [ObservableProperty]
    private List<TinhTrangOption> _tinhTrangs = [];

    [ObservableProperty]
    private ObservableCollection<MonAnItemViewModel> _monAns = [];

    public record LoaiMonAnOption(string MaLoaiMonAn, string TenLoaiMonAn);
    public record DonViTinhOption(string MaDonViTinh, string TenDonViTinh);
    public record TinhTrangOption(string MaTinhTrang, string TenTinhTrang);

    public TraCuuMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        PageSize = 10;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public async Task InitializeFormAsync()
    {
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var rawCategories = await context.LoaiMonAn.AsNoTracking().OrderBy(l => l.TenLoaiMonAn).ToListAsync();
            LoaiMonAns = rawCategories
                .Select(l => new LoaiMonAnOption(l.MaLoaiMonAn, l.TenLoaiMonAn))
                .Prepend(new LoaiMonAnOption("All", "Tất cả"))
                .ToList();

            var rawUnits = await context.DonViTinh.AsNoTracking().OrderBy(u => u.TenDonViTinh).ToListAsync();
            DonViTinhs = rawUnits
                .Select(u => new DonViTinhOption(u.MaDonViTinh, u.TenDonViTinh))
                .Prepend(new DonViTinhOption("All", "Tất cả"))
                .ToList();

            var rawStatuses = await context.TinhTrang.AsNoTracking().OrderBy(t => t.TenTinhTrang).ToListAsync();
            TinhTrangs = rawStatuses
                .Select(t => new TinhTrangOption(t.MaTinhTrang, t.TenTinhTrang))
                .Prepend(new TinhTrangOption("All", "Tất cả"))
                .ToList();
        }

        SelectedMaLoaiMonAn = "All";
        SelectedMaDonViTinh = "All";
        SelectedMaTinhTrang = "All";
        MaMonAn = "";
        TenMonAn = "";
        DonGiaTu = "";
        DonGiaDen = "";

        _hasSearched = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        _hasSearched = true;
        PageNumber = 1;
        await LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        if (!_hasSearched)
        {
            MonAns = [];
            TotalItems = 0;
            UpdatePaginationInfo();
            return;
        }

        int? searchMaMonAn = int.TryParse(MaMonAn, out int parsedId) ? parsedId : null;

        long? minPrice = null;
        if (long.TryParse(DonGiaTu, out long minVal)) minPrice = minVal;

        long? maxPrice = null;
        if (long.TryParse(DonGiaDen, out long maxVal)) maxPrice = maxVal;

        cancellationToken.ThrowIfCancellationRequested();

        async Task<(List<MonAn> items, int total)> QueryMonAnsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.MonAn
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes()
                .Filter(searchMaMonAn, TenMonAn, SelectedMaLoaiMonAn, SelectedMaDonViTinh, SelectedMaTinhTrang, minPrice, maxPrice);

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(m => m.MaMonAn)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        var (rawMonAns, totalItems) = await QueryMonAnsAsync();
        TotalItems = totalItems;

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<MonAnItemViewModel>(rawMonAns.Count);
        foreach (var monAn in rawMonAns)
        {
            page.Add(new MonAnItemViewModel
            {
                STT = stt++,
                MaMonAn = monAn.MaMonAn,
                TenMonAn = monAn.TenMonAn,
                TenLoaiMonAn = monAn.LoaiMonAn?.TenLoaiMonAn ?? "",
                TenDonViTinh = monAn.DonViTinh?.TenDonViTinh ?? "",
                DonGiaText = $"{monAn.DonGia:N0} VND",
                TenTinhTrang = monAn.TinhTrang?.TenTinhTrang ?? ""
            });
        }

        MonAns = new ObservableCollection<MonAnItemViewModel>(page);
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
