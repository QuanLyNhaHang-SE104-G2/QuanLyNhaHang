using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhBanAnViewModel : EditablePaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "loại bàn";

    [ObservableProperty]
    private int _soChoNgoiToiThieu;

    [ObservableProperty]
    private ObservableCollection<LoaiBanItemViewModel> _loaiBans = [];

    [ObservableProperty]
    private ObservableCollection<LoaiBanItemViewModel> _pagedItems = [];

    public QuyDinhBanAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo quy định bàn ăn: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        SoChoNgoiToiThieu = 2;
        LoaiBans.Clear();
        PagedItems.Clear();
        PageNumbers.Clear();
        TotalInfoText = "";
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();
        await LoadDataAsync();
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        async Task<(List<LoaiBan> loaiBans, int soChoNgoi)> QueryQuyDinhAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var param = await context.ThamSo.FirstOrDefaultAsync(cancellationToken);
            int minSeats = param?.SoChoNgoiToiThieu ?? 2;
            var loaiBansDb = await context.LoaiBan.AsNoTracking().ToListAsync(cancellationToken);
            return (loaiBansDb, minSeats);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (loaiBansDb, minSeats) = await QueryQuyDinhAsync();
        SoChoNgoiToiThieu = minSeats;

        var vms = loaiBansDb.Select((lb, index) => new LoaiBanItemViewModel
        {
            STT = index + 1,
            MaLoaiBan = lb.MaLoaiBan,
            TenLoaiBan = lb.TenLoaiBan,
            PhuThu = lb.PhuThu,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        LoaiBans = new ObservableCollection<LoaiBanItemViewModel>(vms);
        TotalItems = LoaiBans.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    protected override void OnPageChanged()
    {
        var pageElements = LoaiBans.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<LoaiBanItemViewModel>(pageElements);
    }

    [RelayCommand]
    private void AddLoaiBan()
    {
        int count = LoaiBans.Count;
        LoaiBans.Add(new LoaiBanItemViewModel
        {
            STT = count + 1,
            MaLoaiBan = $"LB{count + 1}",
            TenLoaiBan = "Loại bàn mới",
            PhuThu = 0,
            IsReadOnly = false,
            IsNew = true
        });
        TotalItems = LoaiBans.Count;
        UpdatePaginationInfo();

        int targetPage = (int)Math.Ceiling((double)TotalItems / PageSize);
        if (PageNumber != targetPage)
        {
            PageNumber = targetPage;
        }
        else
        {
            OnPageChanged();
        }
    }

    [RelayCommand]
    private void DeleteLoaiBan(LoaiBanItemViewModel item)
    {
        LoaiBans.Remove(item);
        for (int i = 0; i < LoaiBans.Count; i++)
        {
            LoaiBans[i].STT = i + 1;
        }
        TotalItems = LoaiBans.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    [RelayCommand]
    private void EditLoaiBan(LoaiBanItemViewModel item)
    {
        item.IsReadOnly = !item.IsReadOnly;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        if (SoChoNgoiToiThieu < 1)
        {
            MessageBox.Show("Số chỗ ngồi tối thiểu phải lớn hơn 0.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (var item in LoaiBans)
        {
            if (string.IsNullOrWhiteSpace(item.TenLoaiBan))
            {
                MessageBox.Show("Tên loại bàn không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (item.PhuThu < 0)
            {
                MessageBox.Show("Phụ thu không được âm.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(item.MaLoaiBan))
            {
                MessageBox.Show("Mã loại bàn không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var param = await context.ThamSo.FirstOrDefaultAsync();
            if (param == null)
            {
                param = new ThamSo { Id = 1, SoChoNgoiToiThieu = SoChoNgoiToiThieu };
                context.ThamSo.Add(param);
            }
            else
            {
                param.SoChoNgoiToiThieu = SoChoNgoiToiThieu;
            }

            var currentDbLoaiBans = await context.LoaiBan.ToListAsync();

            var keptKeys = LoaiBans.Select(x => x.MaLoaiBan).ToHashSet();
            var toDelete = currentDbLoaiBans.Where(x => !keptKeys.Contains(x.MaLoaiBan)).ToList();
            if (toDelete.Count > 0)
            {
                context.LoaiBan.RemoveRange(toDelete);
            }

            foreach (var item in LoaiBans)
            {
                var existing = currentDbLoaiBans.FirstOrDefault(x => x.MaLoaiBan == item.MaLoaiBan);
                if (existing == null)
                {
                    context.LoaiBan.Add(new LoaiBan
                    {
                        MaLoaiBan = item.MaLoaiBan,
                        TenLoaiBan = item.TenLoaiBan,
                        PhuThu = item.PhuThu
                    });
                }
                else
                {
                    existing.TenLoaiBan = item.TenLoaiBan;
                    existing.PhuThu = item.PhuThu;
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi quy định bàn ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
