using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhLoaiMonAnViewModel : EditablePaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "loại món ăn";

    [ObservableProperty]
    private ObservableCollection<LoaiMonAnItemViewModel> _loaiMonAns = [];

    [ObservableProperty]
    private ObservableCollection<LoaiMonAnItemViewModel> _pagedItems = [];

    public QuyDinhLoaiMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo quy định loại món ăn: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        LoaiMonAns.Clear();
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
        async Task<List<LoaiMonAn>> QueryLoaiMonAnsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.LoaiMonAn.AsNoTracking().ToListAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var dbItems = await QueryLoaiMonAnsAsync();

        var vms = dbItems.Select((lma, index) => new LoaiMonAnItemViewModel
        {
            STT = index + 1,
            MaLoaiMonAn = lma.MaLoaiMonAn,
            TenLoaiMonAn = lma.TenLoaiMonAn,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        LoaiMonAns = new ObservableCollection<LoaiMonAnItemViewModel>(vms);
        TotalItems = LoaiMonAns.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    protected override void OnPageChanged()
    {
        var pageElements = LoaiMonAns.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<LoaiMonAnItemViewModel>(pageElements);
    }

    [RelayCommand]
    private void AddLoaiMonAn()
    {
        int count = LoaiMonAns.Count;
        LoaiMonAns.Add(new LoaiMonAnItemViewModel
        {
            STT = count + 1,
            MaLoaiMonAn = $"LMA{count + 1}",
            TenLoaiMonAn = "Loại món mới",
            IsReadOnly = false,
            IsNew = true
        });
        TotalItems = LoaiMonAns.Count;
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
    private void DeleteLoaiMonAn(LoaiMonAnItemViewModel item)
    {
        LoaiMonAns.Remove(item);
        for (int i = 0; i < LoaiMonAns.Count; i++)
        {
            LoaiMonAns[i].STT = i + 1;
        }
        TotalItems = LoaiMonAns.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    [RelayCommand]
    private void EditLoaiMonAn(LoaiMonAnItemViewModel item)
    {
        item.IsReadOnly = !item.IsReadOnly;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        foreach (var item in LoaiMonAns)
        {
            if (string.IsNullOrWhiteSpace(item.TenLoaiMonAn))
            {
                MessageBox.Show("Tên loại món ăn không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(item.MaLoaiMonAn))
            {
                MessageBox.Show("Mã loại món ăn không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var currentDbLoaiMonAns = await context.LoaiMonAn.ToListAsync();

            var keptKeys = LoaiMonAns.Select(x => x.MaLoaiMonAn).ToHashSet();
            var toDelete = currentDbLoaiMonAns.Where(x => !keptKeys.Contains(x.MaLoaiMonAn)).ToList();
            if (toDelete.Count > 0)
            {
                context.LoaiMonAn.RemoveRange(toDelete);
            }

            foreach (var item in LoaiMonAns)
            {
                var existing = currentDbLoaiMonAns.FirstOrDefault(x => x.MaLoaiMonAn == item.MaLoaiMonAn);
                if (existing == null)
                {
                    context.LoaiMonAn.Add(new LoaiMonAn
                    {
                        MaLoaiMonAn = item.MaLoaiMonAn,
                        TenLoaiMonAn = item.TenLoaiMonAn
                    });
                }
                else
                {
                    existing.TenLoaiMonAn = item.TenLoaiMonAn;
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi quy định loại món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
