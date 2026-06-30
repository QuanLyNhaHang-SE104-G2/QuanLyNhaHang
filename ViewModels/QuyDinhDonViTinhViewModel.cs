using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhDonViTinhViewModel : EditablePaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "đơn vị tính";

    [ObservableProperty]
    private ObservableCollection<DonViTinhItemViewModel> _donViTinhs = [];

    [ObservableProperty]
    private ObservableCollection<DonViTinhItemViewModel> _pagedItems = [];

    public QuyDinhDonViTinhViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo quy định đơn vị tính: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        DonViTinhs.Clear();
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
        async Task<List<DonViTinh>> QueryDonViTinhsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.DonViTinh.AsNoTracking().ToListAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var dbItems = await QueryDonViTinhsAsync();

        var vms = dbItems.Select((dvt, index) => new DonViTinhItemViewModel
        {
            STT = index + 1,
            MaDonViTinh = dvt.MaDonViTinh,
            TenDonViTinh = dvt.TenDonViTinh,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        DonViTinhs = new ObservableCollection<DonViTinhItemViewModel>(vms);
        TotalItems = DonViTinhs.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    protected override void OnPageChanged()
    {
        var pageElements = DonViTinhs.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<DonViTinhItemViewModel>(pageElements);
    }

    [RelayCommand]
    private void AddDonViTinh()
    {
        int count = DonViTinhs.Count;
        DonViTinhs.Add(new DonViTinhItemViewModel
        {
            STT = count + 1,
            MaDonViTinh = $"DVT{count + 1}",
            TenDonViTinh = "Đơn vị mới",
            IsReadOnly = false,
            IsNew = true
        });
        TotalItems = DonViTinhs.Count;
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
    private void DeleteDonViTinh(DonViTinhItemViewModel item)
    {
        DonViTinhs.Remove(item);
        for (int i = 0; i < DonViTinhs.Count; i++)
        {
            DonViTinhs[i].STT = i + 1;
        }
        TotalItems = DonViTinhs.Count;
        UpdatePaginationInfo();
        OnPageChanged();
    }

    [RelayCommand]
    private void EditDonViTinh(DonViTinhItemViewModel item)
    {
        item.IsReadOnly = !item.IsReadOnly;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        foreach (var item in DonViTinhs)
        {
            if (string.IsNullOrWhiteSpace(item.TenDonViTinh))
            {
                MessageBox.Show("Tên đơn vị tính không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(item.MaDonViTinh))
            {
                MessageBox.Show("Mã đơn vị tính không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var currentDbDonViTinhs = await context.DonViTinh.ToListAsync();

            var keptKeys = DonViTinhs.Select(x => x.MaDonViTinh).ToHashSet();
            var toDelete = currentDbDonViTinhs.Where(x => !keptKeys.Contains(x.MaDonViTinh)).ToList();
            if (toDelete.Count > 0)
            {
                context.DonViTinh.RemoveRange(toDelete);
            }

            foreach (var item in DonViTinhs)
            {
                var existing = currentDbDonViTinhs.FirstOrDefault(x => x.MaDonViTinh == item.MaDonViTinh);
                if (existing == null)
                {
                    context.DonViTinh.Add(new DonViTinh
                    {
                        MaDonViTinh = item.MaDonViTinh,
                        TenDonViTinh = item.TenDonViTinh
                    });
                }
                else
                {
                    existing.TenDonViTinh = item.TenDonViTinh;
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi quy định đơn vị tính thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
