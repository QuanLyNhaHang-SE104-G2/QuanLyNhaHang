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

public partial class MonAnViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IDialogService _dialogService;

    protected override string EntityLabel => "món";

    [ObservableProperty]
    private ObservableCollection<MonAnItemViewModel> _monAns = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteMonAnCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditMonAnCommand))]
    private MonAnItemViewModel? _selectedMonAn;

    private bool CanDeleteOrEdit => SelectedMonAn != null;

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

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task DeleteMonAnAsync()
    {
        if (SelectedMonAn == null) return;

        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa món ăn '{SelectedMonAn.TenMonAn}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                bool hasOrders = await context.CTGoiMon.AnyAsync(c => c.MaMonAn == SelectedMonAn.MaMonAn);
                if (hasOrders)
                {
                    MessageBox.Show($"Không thể xóa món ăn '{SelectedMonAn.TenMonAn}' vì món ăn đã được gọi trong phiếu gọi món.", "Lỗi xóa món ăn", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var monAn = await context.MonAn.FirstOrDefaultAsync(m => m.MaMonAn == SelectedMonAn.MaMonAn);
                if (monAn != null)
                {
                    context.MonAn.Remove(monAn);
                    await context.SaveChangesAsync();
                }
            }

            MessageBox.Show("Xóa món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedMonAn = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteOrEdit))]
    private async Task EditMonAnAsync(System.Windows.Window? owner)
    {
        if (SelectedMonAn == null) return;

        if (_dialogService.ShowCapNhatMonAnDialog(owner, SelectedMonAn.MaMonAn) == true)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private void SearchMonAn(System.Windows.Window? owner)
    {
        _dialogService.ShowTraCuuMonAnDialog(owner);
    }
}
