using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.ViewModels;

public partial class SoDoBanViewModel : ObservableObject
{
    private readonly AppDbContext _context;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<BanViewModel> _bans = [];

    [ObservableProperty]
    private int _pageSize = 10;

    [ObservableProperty]
    private int _pageNumber = 1;

    [ObservableProperty]
    private int _totalBans;

    [ObservableProperty]
    private string _totalInfoText = "";

    [ObservableProperty]
    private ObservableCollection<int> _pageNumbers = [];

    public int[] PageSizes { get; } = [10, 25, 50, 100];

    public SoDoBanViewModel(AppDbContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        // Get total count
        TotalBans = await _context.Ban.CountAsync();

        // Calculate pages
        int totalPages = (int)Math.Ceiling((double)TotalBans / PageSize);
        if (totalPages < 1) totalPages = 1;

        if (PageNumbers.Count != totalPages)
        {
            PageNumbers.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                PageNumbers.Add(i);
            }
        }

        if (PageNumber < 1)
        {
            PageNumber = 1;
        }
        else if (PageNumber > totalPages)
        {
            PageNumber = totalPages;
        }

        // Fetch paged data
        var rawBans = await _context.Ban
            .Include(b => b.LoaiBan)
            .OrderBy(b => b.MaBan)
            .GetPage(PageNumber, PageSize)
            .ToListAsync();

        Bans.Clear();
        int stt = (PageNumber - 1) * PageSize + 1;
        foreach (var ban in rawBans)
        {
            Bans.Add(new BanViewModel
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

        int start = TotalBans == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
        int end = Math.Min(PageNumber * PageSize, TotalBans);
        TotalInfoText = $"Hiển thị {start}-{end} trên tổng số {TotalBans} bàn";
    }

    [RelayCommand]
    private async Task PrevPageAsync()
    {
        if (PageNumber > 1)
        {
            PageNumber--;
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        int totalPages = (int)Math.Ceiling((double)TotalBans / PageSize);
        if (PageNumber < totalPages)
        {
            PageNumber++;
            await LoadDataAsync();
        }
    }

    partial void OnPageSizeChanged(int value)
    {
        PageNumber = 1;
        _ = LoadDataAsync();
    }

    partial void OnPageNumberChanged(int value)
    {
        if (value >= 1)
        {
            _ = LoadDataAsync();
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
