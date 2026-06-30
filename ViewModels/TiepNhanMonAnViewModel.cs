using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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

public partial class TiepNhanMonAnViewModel : InMemoryPaginatedViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    protected override string EntityLabel => "món ăn";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn loại món ăn.")]
    private string _selectedMaLoaiMonAn = "";

    [ObservableProperty]
    private List<LoaiMonAn> _loaiMonAns = [];

    [ObservableProperty]
    private List<DonViTinh> _donViTinhs = [];

    [ObservableProperty]
    private List<DonViTinh> _allowedDonViTinhs = [];

    [ObservableProperty]
    private List<TinhTrang> _tinhTrangs = [];

    [ObservableProperty]
    private ObservableCollection<TiepNhanMonAnItemViewModel> _dishes = [];

    [ObservableProperty]
    private ObservableCollection<TiepNhanMonAnItemViewModel> _pagedItems = [];

    [ObservableProperty]
    private TiepNhanMonAnItemViewModel? _selectedMonAn;

    private List<(string MaLoaiMonAn, string MaDonViTinh)> _allLoaiMonAnDvt = [];

    public TiepNhanMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        SelectedMaLoaiMonAn = "";
        Dishes.Clear();
        PagedItems.Clear();
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            LoaiMonAns = await context.LoaiMonAn.AsNoTracking().ToListAsync();
            DonViTinhs = await context.DonViTinh.AsNoTracking().ToListAsync();
            TinhTrangs = await context.TinhTrang.AsNoTracking().ToListAsync();
            _allLoaiMonAnDvt = await context.LoaiMonAnDonViTinh
                .AsNoTracking()
                .Select(q => new { q.MaLoaiMonAn, q.MaDonViTinh })
                .ToListAsync()
                .ContinueWith(t => t.Result.Select(x => (x.MaLoaiMonAn, x.MaDonViTinh)).ToList());
        }

        if (LoaiMonAns.Count > 0)
            SelectedMaLoaiMonAn = LoaiMonAns[0].MaLoaiMonAn;

        UpdateAllowedDonViTinhs(SelectedMaLoaiMonAn);
        OnPageChanged();
    }

    private void UpdateAllowedDonViTinhs(string? maLoaiMonAn)
    {
        if (string.IsNullOrEmpty(maLoaiMonAn))
        {
            AllowedDonViTinhs = DonViTinhs;
            return;
        }

        var allowedDvtIds = _allLoaiMonAnDvt
            .Where(q => q.MaLoaiMonAn == maLoaiMonAn)
            .Select(q => q.MaDonViTinh)
            .ToList();

        AllowedDonViTinhs = DonViTinhs.Where(d => allowedDvtIds.Contains(d.MaDonViTinh)).ToList();

        if (Dishes != null)
        {
            foreach (var row in Dishes)
            {
                if (!AllowedDonViTinhs.Any(d => d.MaDonViTinh == row.SelectedMaDonViTinh))
                {
                    row.SelectedMaDonViTinh = AllowedDonViTinhs.FirstOrDefault()?.MaDonViTinh ?? "";
                }
            }
        }
    }

    partial void OnSelectedMaLoaiMonAnChanged(string value)
    {
        UpdateAllowedDonViTinhs(value);
    }

    protected override void OnPageChanged()
    {
        TotalItems = Dishes.Count;
        UpdatePaginationInfo();

        var pageElements = Dishes.GetPage(PageNumber, PageSize).ToList();
        PagedItems = new ObservableCollection<TiepNhanMonAnItemViewModel>(pageElements);
    }

    [RelayCommand]
    private void AddRow()
    {
        var defaultDvt = AllowedDonViTinhs.FirstOrDefault()?.MaDonViTinh ?? "";
        var defaultTinhTrang = TinhTrangs.FirstOrDefault()?.MaTinhTrang ?? "DangBan";

        var rowVm = new TiepNhanMonAnItemViewModel
        {
            STT = Dishes.Count + 1,
            TenMonAn = "",
            SelectedMaDonViTinh = defaultDvt,
            SelectedMaTinhTrang = defaultTinhTrang,
            DonGia = ""
        };
        Dishes.Add(rowVm);

        int targetPage = (int)Math.Ceiling((double)Dishes.Count / PageSize);
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
    private void DeleteRow(TiepNhanMonAnItemViewModel? row)
    {
        if (row != null)
        {
            Dishes.Remove(row);

            int stt = 1;
            foreach (var item in Dishes)
            {
                item.STT = stt++;
            }

            OnPageChanged();
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await InitializeFormAsync();
    }

    [RelayCommand]
    private async Task AcceptAsync(Window? window)
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var errors = GetErrors().Select(e => e.ErrorMessage).ToList();
            MessageBox.Show(string.Join("\n", errors), "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Dishes.Count == 0)
        {
            MessageBox.Show("Vui lòng thêm ít nhất một món ăn.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        foreach (var row in Dishes)
        {
            row.ValidateRow();
            if (row.HasErrors)
            {
                MessageBox.Show("Vui lòng sửa các lỗi nhập liệu trong danh sách món ăn.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using var context = await _dbContextFactory.CreateDbContextAsync();

                int? maxId = await context.MonAn
                    .Select(m => (int?)m.MaMonAn)
                    .MaxAsync();
                
                int nextId = (maxId ?? 0) + 1;

                foreach (var row in Dishes)
                {
                    var newMonAn = new MonAn
                    {
                        MaMonAn = nextId++,
                        TenMonAn = row.TenMonAn,
                        DonGia = long.Parse(row.DonGia),
                        MaLoaiMonAn = SelectedMaLoaiMonAn,
                        MaDonViTinh = row.SelectedMaDonViTinh,
                        MaTinhTrang = row.SelectedMaTinhTrang
                    };
                    await context.MonAn.AddAsync(newMonAn);
                }

                await context.SaveChangesAsync();

                MessageBox.Show("Thêm món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        MessageBox.Show("Không thể lưu món ăn do xung đột mã liên tục. Vui lòng thử lại.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    [RelayCommand]
    private void Cancel(Window? window)
    {
        if (window != null)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
