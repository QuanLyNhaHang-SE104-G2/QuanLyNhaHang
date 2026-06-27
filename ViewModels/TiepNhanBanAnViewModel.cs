using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanBanAnViewModel : ObservableValidator
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private string _maBan = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Tên bàn ăn không được để trống.")]
    private string _tenBan = "";

    [ObservableProperty]
    [CustomValidation(typeof(TiepNhanBanAnViewModel), nameof(ValidateSoChoNgoi))]
    private string _soChoNgoi = "";

    [ObservableProperty]
    private int _soChoNgoiToiThieu;

    [ObservableProperty]
    [Required(ErrorMessage = "Khu vực không được để trống.")]
    private string _khuVuc = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn loại bàn.")]
    private string _selectedMaLoaiBan = "";

    [ObservableProperty]
    private List<LoaiBan> _loaiBans = [];

    [ObservableProperty]
    private string _phuThuText = "0 VNĐ";

    private decimal _phuThuVal = 0;

    public TiepNhanBanAnViewModel(AppDbContext context)
    {
        _context = context;
        _ = ResetFieldsAsync();
    }

    private async Task ResetFieldsAsync()
    {
        TenBan = "";
        SoChoNgoi = "";
        KhuVuc = "";

        // Load parameter SoChoNgoiToiThieu
        var thamSo = await _context.ThamSo.FirstOrDefaultAsync();
        SoChoNgoiToiThieu = thamSo?.SoChoNgoiToiThieu ?? 2;

        // Load LoaiBans
        var rawLoaiBans = await _context.LoaiBan.ToListAsync();
        LoaiBans = rawLoaiBans.OrderBy(l => l.PhuThu).ToList();

        // Select default
        if (LoaiBans.Count > 0)
        {
            SelectedMaLoaiBan = LoaiBans[0].MaLoaiBan;
            UpdatePhuThu(LoaiBans[0]);
        }

        // Generate next MaBan
        await GenerateNextMaBanAsync();
    }

    private async Task GenerateNextMaBanAsync()
    {
        var existingIds = await _context.Ban.Select(b => b.MaBan).ToListAsync();
        int maxId = 0;
        foreach (var id in existingIds)
        {
            if (int.TryParse(id, out int num))
            {
                if (num > maxId) maxId = num;
            }
        }
        MaBan = $"{(maxId + 1):D2}";
    }

    private void UpdatePhuThu(LoaiBan? loaiBan)
    {
        if (loaiBan != null)
        {
            _phuThuVal = loaiBan.PhuThu;
            PhuThuText = $"{_phuThuVal:N0} VNĐ";
        }
        else
        {
            _phuThuVal = 0;
            PhuThuText = "0 VNĐ";
        }
    }

    partial void OnSelectedMaLoaiBanChanged(string value)
    {
        var loaiBan = LoaiBans.FirstOrDefault(l => l.MaLoaiBan == value);
        UpdatePhuThu(loaiBan);
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await ResetFieldsAsync();
    }

    public static ValidationResult? ValidateSoChoNgoi(string value, ValidationContext context)
    {
        var instance = (TiepNhanBanAnViewModel)context.ObjectInstance;
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult("Số chỗ ngồi không được để trống.");
        }
        if (!int.TryParse(value, out int seats))
        {
            return new ValidationResult("Số chỗ ngồi phải là số nguyên hợp lệ.");
        }
        if (seats < instance.SoChoNgoiToiThieu)
        {
            return new ValidationResult($"Số chỗ ngồi phải lớn hơn hoặc bằng số chỗ ngồi tối thiểu ({instance.SoChoNgoiToiThieu}).");
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

        int seats = int.Parse(SoChoNgoi);

        // Check if MaBan already exists
        bool exists = await _context.Ban.AnyAsync(b => b.MaBan == MaBan);
        if (exists)
        {
            MessageBox.Show("Mã bàn ăn đã tồn tại trong hệ thống.", "Lỗi trùng lặp", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            var newBan = new Ban
            {
                MaBan = MaBan,
                TenBan = TenBan,
                KhuVuc = KhuVuc,
                SoChoNgoi = seats,
                MaLoaiBan = SelectedMaLoaiBan
            };

            await _context.Ban.AddAsync(newBan);
            await _context.SaveChangesAsync();

            MessageBox.Show("Tiếp nhận bàn ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
