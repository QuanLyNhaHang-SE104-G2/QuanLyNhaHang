using System;
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

public partial class TiepNhanNhanVienViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private int _maNhanVien;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên nhân viên không được để trống.")]
    private string _tenNhanVien = "";

    public TiepNhanNhanVienViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        InitializeFormAsync().SafeFireAndForget(onError: ex =>
            MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ClearFields()
    {
        TenNhanVien = "";
    }

    public async Task InitializeFormAsync()
    {
        ClearFields();
        await GenerateNextMaNhanVienAsync();
    }

    private async Task GenerateNextMaNhanVienAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        int? maxId = await context.NhanVien
            .Select(n => (int?)n.MaNhanVien)
            .MaxAsync();
        MaNhanVien = (maxId ?? 0) + 1;
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

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var newNv = new NhanVien
                {
                    MaNhanVien = MaNhanVien,
                    TenNhanVien = TenNhanVien
                };

                using (var context = await _dbContextFactory.CreateDbContextAsync())
                {
                    await context.NhanVien.AddAsync(newNv);
                    await context.SaveChangesAsync();
                }

                MessageBox.Show("Tiếp nhận nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
                return;
            }
            catch (DbUpdateException ex) when (ex.IsPrimaryKeyViolation() && attempt < maxRetries - 1)
            {
                await GenerateNextMaNhanVienAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cơ sở dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        MessageBox.Show("Không thể lưu nhân viên do xung đột mã liên tục. Vui lòng thử lại.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
