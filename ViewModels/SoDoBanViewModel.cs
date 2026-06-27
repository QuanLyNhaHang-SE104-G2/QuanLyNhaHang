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
    private readonly AppDbContext _context;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "bàn";

    [ObservableProperty]
    private ObservableCollection<BanItemViewModel> _bans = [];

    public SoDoBanViewModel(AppDbContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
        _ = LoadDataAsync();
    }

    public override async Task LoadDataAsync()
    {
        TotalItems = await _context.Ban.CountAsync();

        UpdatePaginationInfo();

        var rawBans = await _context.Ban
            .GetWithIncludes()
            .OrderBy(b => b.MaBan)
            .GetPage(PageNumber, PageSize)
            .ToListAsync();

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
                PhuThuText = $"{(ban.LoaiBan?.PhuThu ?? 0):N0} VNĐ"
            });
        }
    }

    [RelayCommand]
    private async Task AddTableAsync()
    {
        if (_dialogService.ShowTiepNhanBanAnDialog() == true)
        {
            await LoadDataAsync();
        }
    }
}
