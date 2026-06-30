using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class LoaiMonAnItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maLoaiMonAn = "";

    [ObservableProperty]
    private string _tenLoaiMonAn = "";

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}

public partial class DonViTinhItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maDonViTinh = "";

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}

public partial class QuyDinh3ViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<LoaiMonAnItemViewModel> _loaiMonAns = [];

    [ObservableProperty]
    private ObservableCollection<DonViTinhItemViewModel> _donViTinhs = [];

    public QuyDinh3ViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var loaiMonAnsDb = await context.LoaiMonAn.AsNoTracking().ToListAsync(cancellationToken);
        var vmsMonAn = loaiMonAnsDb.Select((lma, index) => new LoaiMonAnItemViewModel
        {
            STT = index + 1,
            MaLoaiMonAn = lma.MaLoaiMonAn,
            TenLoaiMonAn = lma.TenLoaiMonAn,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        var donViTinhsDb = await context.DonViTinh.AsNoTracking().ToListAsync(cancellationToken);
        var vmsDvt = donViTinhsDb.Select((dvt, index) => new DonViTinhItemViewModel
        {
            STT = index + 1,
            MaDonViTinh = dvt.MaDonViTinh,
            TenDonViTinh = dvt.TenDonViTinh,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        App.Current.Dispatcher.Invoke(() =>
        {
            LoaiMonAns = new ObservableCollection<LoaiMonAnItemViewModel>(vmsMonAn);
            DonViTinhs = new ObservableCollection<DonViTinhItemViewModel>(vmsDvt);
        });
    }

    [RelayCommand]
    private void ThemLoaiMonAn()
    {
        int count = LoaiMonAns.Count;
        LoaiMonAns.Add(new LoaiMonAnItemViewModel
        {
            STT = count + 1,
            MaLoaiMonAn = $"LMA{count + 1}",
            TenLoaiMonAn = "Món mới",
            IsReadOnly = false,
            IsNew = true
        });
    }

    [RelayCommand]
    private void DeleteLoaiMonAn(LoaiMonAnItemViewModel item)
    {
        LoaiMonAns.Remove(item);
        for (int i = 0; i < LoaiMonAns.Count; i++)
        {
            LoaiMonAns[i].STT = i + 1;
        }
    }

    [RelayCommand]
    private void ThemDonViTinh()
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
    }

    [RelayCommand]
    private void DeleteDonViTinh(DonViTinhItemViewModel item)
    {
        DonViTinhs.Remove(item);
        for (int i = 0; i < DonViTinhs.Count; i++)
        {
            DonViTinhs[i].STT = i + 1;
        }
    }

    [RelayCommand]
    private async Task LuuThayDoiAsync()
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            // --- 1. Process LoaiMonAn ---
            var currentDbLoaiMonAns = await context.LoaiMonAn.ToListAsync();

            // Deletions
            var keptLmaKeys = LoaiMonAns.Select(x => x.MaLoaiMonAn).ToHashSet();
            var toDeleteLma = currentDbLoaiMonAns.Where(x => !keptLmaKeys.Contains(x.MaLoaiMonAn)).ToList();
            if (toDeleteLma.Any())
            {
                context.LoaiMonAn.RemoveRange(toDeleteLma);
            }

            // Additions / Updates
            foreach (var item in LoaiMonAns)
            {
                var existing = currentDbLoaiMonAns.FirstOrDefault(x => x.MaLoaiMonAn == item.MaLoaiMonAn);
                if (existing == null)
                {
                    var newLma = new LoaiMonAn
                    {
                        MaLoaiMonAn = item.MaLoaiMonAn,
                        TenLoaiMonAn = item.TenLoaiMonAn
                    };
                    context.LoaiMonAn.Add(newLma);
                }
                else
                {
                    existing.TenLoaiMonAn = item.TenLoaiMonAn;
                    context.LoaiMonAn.Update(existing);
                }
            }

            // --- 2. Process DonViTinh ---
            var currentDbDonViTinhs = await context.DonViTinh.ToListAsync();

            // Deletions
            var keptDvtKeys = DonViTinhs.Select(x => x.MaDonViTinh).ToHashSet();
            var toDeleteDvt = currentDbDonViTinhs.Where(x => !keptDvtKeys.Contains(x.MaDonViTinh)).ToList();
            if (toDeleteDvt.Any())
            {
                context.DonViTinh.RemoveRange(toDeleteDvt);
            }

            // Additions / Updates
            foreach (var item in DonViTinhs)
            {
                var existing = currentDbDonViTinhs.FirstOrDefault(x => x.MaDonViTinh == item.MaDonViTinh);
                if (existing == null)
                {
                    var newDvt = new DonViTinh
                    {
                        MaDonViTinh = item.MaDonViTinh,
                        TenDonViTinh = item.TenDonViTinh
                    };
                    context.DonViTinh.Add(newDvt);
                }
                else
                {
                    existing.TenDonViTinh = item.TenDonViTinh;
                    context.DonViTinh.Update(existing);
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi Quy định 3 thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lưu thay đổi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
