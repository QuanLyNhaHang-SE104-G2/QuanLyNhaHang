using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.ViewModels;

public partial class MonAnViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "món";

    [ObservableProperty]
    private ObservableCollection<MonAnItemViewModel> _monAns = [];

    [ObservableProperty]
    private MonAnItemViewModel? _selectedMonAn;

    public MonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<MonAn> items, int total)> QueryMonAnsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.MonAn
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes();

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(m => m.MaMonAn)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

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
    private async Task AddMonAnAsync(System.Windows.Window? owner)
    {
        if (_dialogService.ShowTiepNhanMonAnDialog(owner) == true)
        {
            await LoadDataAsync();
        }
    }
}
