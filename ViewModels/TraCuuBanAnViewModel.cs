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

public partial class TraCuuBanAnViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private bool _hasSearched;

    protected override string EntityLabel => "bàn";

    [ObservableProperty]
    private string _maBan = "";

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _khuVuc = "";

    [ObservableProperty]
    private string _selectedMaLoaiBan = "All";

    [ObservableProperty]
    private string _soChoNgoiTu = "";

    [ObservableProperty]
    private string _soChoNgoiDen = "";

    [ObservableProperty]
    private string _phuThuTu = "";

    [ObservableProperty]
    private string _phuThuDen = "";

    [ObservableProperty]
    private ObservableCollection<LoaiBanOption> _loaiBans = [];

    [ObservableProperty]
    private ObservableCollection<BanItemViewModel> _bans = [];

    public record LoaiBanOption(string MaLoaiBan, string TenLoaiBan);

    public TraCuuBanAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public async Task InitializeFormAsync()
    {
        async Task<List<LoaiBanOption>> GetAllLoaiBansAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.LoaiBan
                .AsNoTracking()
                .OrderBy(l => l.PhuThu)
                .Select(l => new LoaiBanOption(l.MaLoaiBan, l.TenLoaiBan))
                .ToListAsync();
        }

        var rawLoaiBans = await GetAllLoaiBansAsync();

        LoaiBans = new ObservableCollection<LoaiBanOption>(
            rawLoaiBans.Prepend(new LoaiBanOption("All", "Tất cả")));

        SelectedMaLoaiBan = "All";

        MaBan = "";
        TenBan = "";
        KhuVuc = "";
        SoChoNgoiTu = "";
        SoChoNgoiDen = "";
        PhuThuTu = "";
        PhuThuDen = "";

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
            Bans = [];
            TotalItems = 0;
            UpdatePaginationInfo();
            return;
        }

        int? searchMaBan = int.TryParse(MaBan, out int parsedMaBan) ? parsedMaBan : null;
        int? minSeats = int.TryParse(SoChoNgoiTu, out int minS) ? minS : null;
        int? maxSeats = int.TryParse(SoChoNgoiDen, out int maxS) ? maxS : null;
        long? minPhuThu = long.TryParse(PhuThuTu, out long minP) ? minP : null;
        long? maxPhuThu = long.TryParse(PhuThuDen, out long maxP) ? maxP : null;

        List<Ban> rawBans;

        using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = context.Ban
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes()
                .Filter(searchMaBan, TenBan, KhuVuc, SelectedMaLoaiBan, minSeats, maxSeats, minPhuThu, maxPhuThu);

            TotalItems = await query.CountAsync(cancellationToken);
            UpdatePaginationInfo();

            rawBans = await query
                .OrderBy(b => b.MaBan)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);
        }

        var tempList = new List<BanItemViewModel>();
        int stt = (PageNumber - 1) * PageSize + 1;

        foreach (var ban in rawBans)
        {
            tempList.Add(new BanItemViewModel
            {
                STT = stt++,
                MaBan = ban.MaBan,
                TenBan = ban.TenBan,
                KhuVuc = ban.KhuVuc,
                SoChoNgoi = ban.SoChoNgoi,
                SoChoNgoiText = $"{ban.SoChoNgoi} khách",
                TenLoaiBan = ban.LoaiBan?.TenLoaiBan ?? "",
                PhuThuText = $"{(ban.LoaiBan?.PhuThu ?? 0):N0} VND"
            });
        }

        Bans = new ObservableCollection<BanItemViewModel>(tempList);
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
