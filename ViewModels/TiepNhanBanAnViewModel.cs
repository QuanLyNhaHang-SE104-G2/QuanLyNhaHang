using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Extensions;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanBanAnViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private int _maBan;

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
    private string _phuThuText = "0 VND";

    private decimal _phuThuVal = 0;

    public TiepNhanBanAnViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        TenBan = "";
        SoChoNgoi = "";
        KhuVuc = "";
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();

        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            SoChoNgoiToiThieu = await context.GetSoChoNgoiToiThieuAsync();
            LoaiBans = await context.LoaiBan.AsNoTracking().OrderBy(l => l.PhuThu).ToListAsync();
        }

        if (LoaiBans.Count > 0)
        {
            SelectedMaLoaiBan = LoaiBans[0].MaLoaiBan;
            UpdatePhuThu(LoaiBans[0]);
        }

        await GenerateNextMaBanAsync();
    }

    private async Task GenerateNextMaBanAsync()
    {
        async Task<int> GetMaxIdAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            int? maxId = await context.Ban
                .Select(m => (int?)m.MaBan)
                .MaxAsync();

            return maxId ?? 0;
        }
        int maxId = await GetMaxIdAsync();
        MaBan = maxId + 1;
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
        await InitializeFormAsync();
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

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
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

                using (var context = await _dbContextFactory.CreateDbContextAsync())
                {
                    await context.Ban.AddAsync(newBan);
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Tiếp nhận bàn ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
                // PK collision (TOCTOU): another insert took our ID.
                // Re-generate and retry.
                await GenerateNextMaBanAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        // Exhausted retries
        MessageBox.Show("Không thể lưu bàn ăn do xung đột mã liên tục. Vui lòng thử lại.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
