using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanMonAnViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private int _maMonAn;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên món ăn không được để trống.")]
    private string _tenMonAn = "";

    [ObservableProperty]
    [CustomValidation(typeof(TiepNhanMonAnViewModel), nameof(ValidateDonGia))]
    private string _donGia = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn loại món ăn.")]
    private string _selectedMaLoaiMonAn = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn đơn vị tính.")]
    private string _selectedMaDonViTinh = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn tình trạng.")]
    private string _selectedMaTinhTrang = "";

    [ObservableProperty]
    private List<LoaiMonAn> _loaiMonAns = [];

    [ObservableProperty]
    private List<DonViTinh> _donViTinhs = [];

    [ObservableProperty]
    private List<DonViTinh> _allowedDonViTinhs = [];

    [ObservableProperty]
    private List<TinhTrang> _tinhTrangs = [];

    public TiepNhanMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _ = InitializeFormAsync();
    }

    private void ClearFields()
    {
        TenMonAn = "";
        DonGia = "";
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();

        // Batching checks to reduce context creation round trips and lifetime
        if (LoaiMonAns.Count == 0 || DonViTinhs.Count == 0 || TinhTrangs.Count == 0)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            if (LoaiMonAns.Count == 0)
                LoaiMonAns = await context.LoaiMonAn.AsNoTracking().ToListAsync();
            if (DonViTinhs.Count == 0)
                DonViTinhs = await context.DonViTinh.AsNoTracking().ToListAsync();
            if (TinhTrangs.Count == 0)
                TinhTrangs = await context.TinhTrang.AsNoTracking().ToListAsync();
        }

        if (LoaiMonAns.Count > 0)
            SelectedMaLoaiMonAn = LoaiMonAns[0].MaLoaiMonAn;

        if (TinhTrangs.Count > 0)
            SelectedMaTinhTrang = TinhTrangs[0].MaTinhTrang;

        await UpdateAllowedDonViTinhsAsync(SelectedMaLoaiMonAn);
        await GenerateNextMaMonAnAsync();
    }

    private async Task GenerateNextMaMonAnAsync()
    {
        async Task<int> GetMaxIdAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            int? maxId = await context.MonAn
                .Select(m => (int?)m.MaMonAn)
                .MaxAsync();

            return maxId ?? 0;
        }

        int maxId = await GetMaxIdAsync();
        MaMonAn = maxId + 1;
    }

    private async Task UpdateAllowedDonViTinhsAsync(string? maLoaiMonAn)
    {
        async Task<List<string>> GetAllDonViTinhsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.LoaiMonAnDonViTinh
                .AsNoTracking()
                .Where(q => q.MaLoaiMonAn == maLoaiMonAn)
                .Select(q => q.MaDonViTinh)
                .ToListAsync();
        }

        if (string.IsNullOrEmpty(maLoaiMonAn))
        {
            AllowedDonViTinhs = DonViTinhs;
            return;
        }

        var allowedDvtIds = await GetAllDonViTinhsAsync();

        AllowedDonViTinhs = DonViTinhs.Where(d => allowedDvtIds.Contains(d.MaDonViTinh)).ToList();

        if (!AllowedDonViTinhs.Any(d => d.MaDonViTinh == SelectedMaDonViTinh))
        {
            SelectedMaDonViTinh = AllowedDonViTinhs.FirstOrDefault()?.MaDonViTinh ?? "";
        }
    }

    partial void OnSelectedMaLoaiMonAnChanged(string value)
    {
        _ = UpdateAllowedDonViTinhsAsync(value);
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await InitializeFormAsync();
    }

    public static ValidationResult? ValidateDonGia(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value) || !long.TryParse(value, out long donGia) || donGia < 0)
        {
            return new ValidationResult("Đơn giá phải là số nguyên hợp lệ và lớn hơn hoặc bằng 0.");
        }
        return ValidationResult.Success;
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

        long donGia = long.Parse(DonGia);

        var newMonAn = new MonAn
        {
            MaMonAn = MaMonAn,
            TenMonAn = TenMonAn,
            DonGia = donGia,
            MaLoaiMonAn = SelectedMaLoaiMonAn,
            MaDonViTinh = SelectedMaDonViTinh,
            MaTinhTrang = SelectedMaTinhTrang
        };

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                await context.MonAn.AddAsync(newMonAn);
                await context.SaveChangesAsync();
            }
            MessageBox.Show("Thêm món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
