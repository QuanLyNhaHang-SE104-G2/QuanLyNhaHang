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

public partial class CapNhatMonAnViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private int _initialMaMonAn;

    [ObservableProperty]
    private int _maMonAn;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên món ăn không được để trống.")]
    private string _tenMonAn = "";

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
    [CustomValidation(typeof(CapNhatMonAnViewModel), nameof(ValidateDonGia))]
    private string _donGia = "";

    [ObservableProperty]
    private List<LoaiMonAn> _loaiMonAns = [];

    [ObservableProperty]
    private List<DonViTinh> _donViTinhs = [];

    [ObservableProperty]
    private List<DonViTinh> _allowedDonViTinhs = [];

    [ObservableProperty]
    private List<TinhTrang> _tinhTrangs = [];

    private List<(string MaLoaiMonAn, string MaDonViTinh)> _allLoaiMonAnDvt = [];

    public CapNhatMonAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    partial void OnMaMonAnChanged(int value)
    {
        _ = LoadMonAnAsync(value);
    }

    public async Task LoadMonAnAsync(int maMonAn)
    {
        _initialMaMonAn = maMonAn;

        try
        {
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

                var monAn = await context.MonAn.AsNoTracking().FirstOrDefaultAsync(m => m.MaMonAn == maMonAn);
                if (monAn != null)
                {
                    TenMonAn = monAn.TenMonAn;
                    SelectedMaLoaiMonAn = monAn.MaLoaiMonAn;
                    UpdateAllowedDonViTinhs(monAn.MaLoaiMonAn);
                    SelectedMaDonViTinh = monAn.MaDonViTinh;
                    SelectedMaTinhTrang = monAn.MaTinhTrang;
                    DonGia = monAn.DonGia.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin món ăn: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

        if (!AllowedDonViTinhs.Any(d => d.MaDonViTinh == SelectedMaDonViTinh))
        {
            SelectedMaDonViTinh = AllowedDonViTinhs.FirstOrDefault()?.MaDonViTinh ?? "";
        }
    }

    partial void OnSelectedMaLoaiMonAnChanged(string value)
    {
        UpdateAllowedDonViTinhs(value);
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await LoadMonAnAsync(_initialMaMonAn);
    }

    public static ValidationResult? ValidateDonGia(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult("Đơn giá không được để trống.");
        }
        if (!long.TryParse(value, out long price))
        {
            return new ValidationResult("Đơn giá phải là số nguyên hợp lệ.");
        }
        if (price < 0)
        {
            return new ValidationResult("Đơn giá không được nhỏ hơn 0.");
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

        long price = long.Parse(DonGia);

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var existingMonAn = await context.MonAn.FirstOrDefaultAsync(m => m.MaMonAn == MaMonAn);
                if (existingMonAn == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn cần cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                existingMonAn.TenMonAn = TenMonAn;
                existingMonAn.DonGia = price;
                existingMonAn.MaLoaiMonAn = SelectedMaLoaiMonAn;
                existingMonAn.MaDonViTinh = SelectedMaDonViTinh;
                existingMonAn.MaTinhTrang = SelectedMaTinhTrang;

                await context.SaveChangesAsync();
            }

            MessageBox.Show("Cập nhật thông tin món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
