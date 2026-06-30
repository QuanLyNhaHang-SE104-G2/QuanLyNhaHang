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

public partial class CapNhatBanAnViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private int _initialMaBan;

    [ObservableProperty]
    private int _maBan;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên bàn ăn không được để trống.")]
    private string _tenBan = "";

    [ObservableProperty]
    [CustomValidation(typeof(CapNhatBanAnViewModel), nameof(ValidateSoChoNgoi))]
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
    private string _phuThuText = "0 VND";

    private decimal _phuThuVal = 0;

    public CapNhatBanAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    partial void OnMaBanChanged(int value)
    {
        _ = LoadBanAsync(value);
    }

    public async Task LoadBanAsync(int maBan)
    {
        _initialMaBan = maBan;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                LoaiBans = await context.LoaiBan.AsNoTracking().OrderBy(l => l.PhuThu).ToListAsync();
                SoChoNgoiToiThieu = await context.GetSoChoNgoiToiThieuAsync();

                var ban = await context.Ban.AsNoTracking().FirstOrDefaultAsync(b => b.MaBan == maBan);
                if (ban != null)
                {
                    TenBan = ban.TenBan;
                    SoChoNgoi = ban.SoChoNgoi.ToString();
                    KhuVuc = ban.KhuVuc;
                    SelectedMaLoaiBan = ban.MaLoaiBan;

                    var loaiBan = LoaiBans.FirstOrDefault(l => l.MaLoaiBan == ban.MaLoaiBan);
                    UpdatePhuThu(loaiBan);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin bàn ăn: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdatePhuThu(LoaiBan? loaiBan)
    {
        if (loaiBan != null)
        {
            _phuThuVal = loaiBan.PhuThu;
            PhuThuText = $"{_phuThuVal:N0} VND";
        }
        else
        {
            _phuThuVal = 0;
            PhuThuText = "0 VND";
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
        await LoadBanAsync(_initialMaBan);
    }

    public static ValidationResult? ValidateSoChoNgoi(string value, ValidationContext context)
    {
        var instance = (CapNhatBanAnViewModel)context.ObjectInstance;
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

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var existingBan = await context.Ban.FirstOrDefaultAsync(b => b.MaBan == MaBan);
                if (existingBan == null)
                {
                    MessageBox.Show("Không tìm thấy bàn ăn cần cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                existingBan.TenBan = TenBan;
                existingBan.KhuVuc = KhuVuc;
                existingBan.SoChoNgoi = seats;
                existingBan.MaLoaiBan = SelectedMaLoaiBan;

                await context.SaveChangesAsync();
            }

            MessageBox.Show("Cập nhật thông tin bàn ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
            return;
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
