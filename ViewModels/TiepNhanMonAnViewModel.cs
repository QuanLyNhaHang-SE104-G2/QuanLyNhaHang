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
    private readonly AppDbContext _context;

    [ObservableProperty]
    private string _maMonAn = "";

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

    public TiepNhanMonAnViewModel(AppDbContext context)
    {
        _context = context;
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

        if (LoaiMonAns.Count == 0)
            LoaiMonAns = await _context.LoaiMonAn.ToListAsync();
        if (DonViTinhs.Count == 0)
            DonViTinhs = await _context.DonViTinh.ToListAsync();
        if (TinhTrangs.Count == 0)
            TinhTrangs = await _context.TinhTrang.ToListAsync();

        if (LoaiMonAns.Count > 0)
            SelectedMaLoaiMonAn = LoaiMonAns[0].MaLoaiMonAn;

        if (TinhTrangs.Count > 0)
            SelectedMaTinhTrang = TinhTrangs[0].MaTinhTrang;

        await UpdateAllowedDonViTinhsAsync(SelectedMaLoaiMonAn);
        await GenerateNextMaMonAnAsync();
    }

    private async Task GenerateNextMaMonAnAsync()
    {
        var existingIds = await _context.MonAn.Select(m => m.MaMonAn).ToListAsync();
        int maxId = 0;
        foreach (var id in existingIds)
        {
            if (int.TryParse(id, out int num))
            {
                if (num > maxId) maxId = num;
            }
        }
        MaMonAn = $"{(maxId + 1):D3}";
    }

    private async Task UpdateAllowedDonViTinhsAsync(string? maLoaiMonAn)
    {
        if (string.IsNullOrEmpty(maLoaiMonAn))
        {
            AllowedDonViTinhs = DonViTinhs;
            return;
        }

        var allowedDvtIds = await _context.LoaiMonAnDonViTinh
            .Where(q => q.MaLoaiMonAn == maLoaiMonAn)
            .Select(q => q.MaDonViTinh)
            .ToListAsync();

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
        if (string.IsNullOrWhiteSpace(value) || !decimal.TryParse(value, out decimal donGia) || donGia < 0)
        {
            return new ValidationResult("Đơn giá phải là số hợp lệ và lớn hơn hoặc bằng 0.");
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

        decimal donGia = decimal.Parse(DonGia);

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
            await _context.MonAn.AddAsync(newMonAn);
            await _context.SaveChangesAsync();
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
