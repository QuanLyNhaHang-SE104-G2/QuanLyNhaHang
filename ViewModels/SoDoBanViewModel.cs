using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteBanCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditBanCommand))]
    private BanItemViewModel? _selectedBan;

    private bool CanDeleteOrEdit => SelectedBan != null;

    public SoDoBanViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
    {
        _dbContextFactory = dbContextFactory;
        _dialogService = dialogService;
        LoadDataAsync().SafeFireAndForget();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<Ban> items, int total)> QueryBansAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.Ban
                .AsNoTrackingWithIdentityResolution()
                .GetWithIncludes();

            int total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(b => b.MaBan)
                .GetPage(PageNumber, PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (rawBans, totalItems) = await QueryBansAsync();
        TotalItems = totalItems;

        UpdatePaginationInfo();

        int stt = (PageNumber - 1) * PageSize + 1;
        var page = new List<BanItemViewModel>(rawBans.Count);
        foreach (var ban in rawBans)
        {
            page.Add(new BanItemViewModel
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

        Bans = new ObservableCollection<BanItemViewModel>(page);
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
    private async Task SearchTableAsync(System.Windows.Window? owner)
    {
        _dialogService.ShowTraCuuBanAnDialog(owner);
        await LoadDataAsync();
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task DeleteBanAsync()
    {
        if (SelectedBan == null) return;

        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bàn ăn '{SelectedBan.TenBan}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                bool hasOrders = await context.PhieuGoiMon.AnyAsync(p => p.MaBan == SelectedBan.MaBan);
                if (hasOrders)
                {
                    MessageBox.Show($"Không thể xóa bàn ăn '{SelectedBan.TenBan}' vì bàn đã có thông tin gọi món.", "Lỗi xóa bàn ăn", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var ban = await context.Ban.FirstOrDefaultAsync(b => b.MaBan == SelectedBan.MaBan);
                if (ban != null)
                {
                    context.Ban.Remove(ban);
                    await context.SaveChangesAsync();
                }
            }

            MessageBox.Show("Xóa bàn ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedBan = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task EditBanAsync(System.Windows.Window? owner)
    {
        if (SelectedBan == null) return;

        if (_dialogService.ShowCapNhatBanAnDialog(owner, SelectedBan.MaBan) == true)
        {
            await LoadDataAsync();
        }
    }
}
