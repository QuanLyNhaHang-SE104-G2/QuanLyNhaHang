using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.ViewModels;

public partial class MonAnViewModel : PaginatedViewModelBase
{
    private readonly AppDbContext _context;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "món";

    [ObservableProperty]
    private ObservableCollection<MonAnItemViewModel> _monAns = [];

    [ObservableProperty]
    private MonAnItemViewModel? _selectedMonAn;

    public MonAnViewModel(AppDbContext context, IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
        _ = LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync()
    {
        TotalItems = await _context.MonAn.CountAsync();

        UpdatePaginationInfo();

        var rawMonAns = await _context.MonAn
            .GetWithIncludes()
            .OrderBy(m => m.MaMonAn)
            .GetPage(PageNumber, PageSize)
            .ToListAsync();

        MonAns.Clear();
        int stt = (PageNumber - 1) * PageSize + 1;
        foreach (var monAn in rawMonAns)
        {
            MonAns.Add(new MonAnItemViewModel
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
    }

    [RelayCommand]
    private async Task AddMonAnAsync(System.Windows.Window? owner)
    {
        if (_dialogService.ShowTiepNhanMonAnDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }
}
