using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.ViewModels;

public partial class SoDoBanViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "bàn";

    [ObservableProperty]
    private ObservableCollection<BanItemViewModel> _bans = [];

    public SoDoBanViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        _ = LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<List<Ban>> GetBansAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Ban
                .GetWithIncludes()
                .OrderBy(b => b.MaBan)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);
        }

        using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            TotalItems = await context.Ban.CountAsync(cancellationToken);
        }

        UpdatePaginationInfo();

        var rawBans = await GetBansAsync();

        Bans.Clear();
        int stt = (PageNumber - 1) * PageSize + 1;
        foreach (var ban in rawBans)
        {
            Bans.Add(new BanItemViewModel
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
    }

    [RelayCommand]
    private async Task AddTableAsync(System.Windows.Window? owner)
    {
        if (_dialogService.ShowTiepNhanBanAnDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private void SearchTable(System.Windows.Window? owner)
    {
        _dialogService.ShowTraCuuBanAnDialog(owner);
    }
}
