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
    private ObservableCollection<LoaiBan> _loaiBans = [];

    [ObservableProperty]
    private ObservableCollection<BanItemViewModel> _bans = [];

    public TraCuuBanAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _ = InitializeFormAsync();
    }

    public async Task InitializeFormAsync()
    {
        async Task<List<LoaiBan>> GetAllLoaiBansAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.LoaiBan.OrderBy(l => l.PhuThu).ToListAsync();
        }

        var rawLoaiBans = await GetAllLoaiBansAsync();

        // Fixed: Single-line fluent composition using Prepend
        LoaiBans = new(rawLoaiBans.Prepend(new LoaiBan
        {
            MaLoaiBan = "All",
            TenLoaiBan = "Tất cả"
        }));

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

        int? minSeats = int.TryParse(SoChoNgoiTu, out int minS) ? minS : null;
        int? maxSeats = int.TryParse(SoChoNgoiDen, out int maxS) ? maxS : null;
        decimal? minPhuThu = decimal.TryParse(PhuThuTu, out decimal minP) ? minP : null;
        decimal? maxPhuThu = decimal.TryParse(PhuThuDen, out decimal maxP) ? maxP : null;

        List<Ban> rawBans;

        using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = context.Ban
                .AsNoTracking() // Using AsNoTracking for read-only operations to improve performance
                .GetWithIncludes()
                .Filter(MaBan, TenBan, KhuVuc, SelectedMaLoaiBan, minSeats, maxSeats, minPhuThu, maxPhuThu);

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
