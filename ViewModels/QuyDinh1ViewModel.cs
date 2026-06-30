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

public partial class LoaiBanItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maLoaiBan = "";

    [ObservableProperty]
    private string _tenLoaiBan = "";

    [ObservableProperty]
    private long _phuThu;

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}

public partial class QuyDinh1ViewModel : PaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private int _soChoNgoiToiThieu;

    [ObservableProperty]
    private ObservableCollection<LoaiBanItemViewModel> _loaiBans = [];

    public QuyDinh1ViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task OnLoadDataAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var ts = await context.ThamSo.FirstOrDefaultAsync(cancellationToken);
        SoChoNgoiToiThieu = ts?.SoChoNgoiToiThieu ?? 2;

        var loaiBansDb = await context.LoaiBan.AsNoTracking().ToListAsync(cancellationToken);
        
        var vms = loaiBansDb.Select((lb, index) => new LoaiBanItemViewModel
        {
            STT = index + 1,
            MaLoaiBan = lb.MaLoaiBan,
            TenLoaiBan = lb.TenLoaiBan,
            PhuThu = lb.PhuThu,
            IsReadOnly = true,
            IsNew = false
        }).ToList();

        App.Current.Dispatcher.Invoke(() =>
        {
            LoaiBans = new ObservableCollection<LoaiBanItemViewModel>(vms);
        });
    }

    [RelayCommand]
    private void ThemLoaiBan()
    {
        int count = LoaiBans.Count;
        LoaiBans.Add(new LoaiBanItemViewModel
        {
            STT = count + 1,
            MaLoaiBan = $"LB{count + 1}",
            TenLoaiBan = "Loại bàn mới",
            PhuThu = 0,
            IsReadOnly = false, // editable by default for new row
            IsNew = true
        });
    }

    [RelayCommand]
    private void EditLoaiBan(LoaiBanItemViewModel item)
    {
        // No-op for Phase 1 as per user request
    }

    [RelayCommand]
    private void DeleteLoaiBan(LoaiBanItemViewModel item)
    {
        LoaiBans.Remove(item);
        // Re-calculate STT
        for (int i = 0; i < LoaiBans.Count; i++)
        {
            LoaiBans[i].STT = i + 1;
        }
    }

    [RelayCommand]
    private async Task LuuThayDoiAsync()
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            
            // 1. Update ThamSo
            var ts = await context.ThamSo.FirstOrDefaultAsync();
            if (ts == null)
            {
                ts = new ThamSo { Id = 1, SoChoNgoiToiThieu = SoChoNgoiToiThieu };
                context.ThamSo.Add(ts);
            }
            else
            {
                ts.SoChoNgoiToiThieu = SoChoNgoiToiThieu;
                context.ThamSo.Update(ts);
            }

            // 2. Update LoaiBan lists
            var currentDbLoaiBans = await context.LoaiBan.ToListAsync();

            // Deletions
            var keptKeys = LoaiBans.Select(x => x.MaLoaiBan).ToHashSet();
            var toDelete = currentDbLoaiBans.Where(x => !keptKeys.Contains(x.MaLoaiBan)).ToList();
            if (toDelete.Any())
            {
                context.LoaiBan.RemoveRange(toDelete);
            }

            // Additions / Updates
            foreach (var item in LoaiBans)
            {
                var existing = currentDbLoaiBans.FirstOrDefault(x => x.MaLoaiBan == item.MaLoaiBan);
                if (existing == null)
                {
                    var newLoaiBan = new LoaiBan
                    {
                        MaLoaiBan = item.MaLoaiBan,
                        TenLoaiBan = item.TenLoaiBan,
                        PhuThu = item.PhuThu
                    };
                    context.LoaiBan.Add(newLoaiBan);
                }
                else
                {
                    existing.TenLoaiBan = item.TenLoaiBan;
                    existing.PhuThu = item.PhuThu;
                    context.LoaiBan.Update(existing);
                }
            }

            await context.SaveChangesAsync();
            MessageBox.Show("Lưu thay đổi Quy định 1 thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lưu thay đổi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
