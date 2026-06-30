using System;
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

public partial class CapNhatNhanVienViewModel : ObservableValidator
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private int _initialMaNhanVien;

    [ObservableProperty]
    private int _maNhanVien;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên nhân viên không được để trống.")]
    private string _tenNhanVien = "";

    public CapNhatNhanVienViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    partial void OnMaNhanVienChanged(int value)
    {
        _ = LoadNhanVienAsync(value);
    }

    public async Task LoadNhanVienAsync(int maNhanVien)
    {
        _initialMaNhanVien = maNhanVien;

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var nv = await context.NhanVien.AsNoTracking().FirstOrDefaultAsync(n => n.MaNhanVien == maNhanVien);
                if (nv != null)
                {
                    TenNhanVien = nv.TenNhanVien;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin nhân viên: {ex.Message}", "Lỗi hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await LoadNhanVienAsync(_initialMaNhanVien);
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

        try
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync())
            {
                var dbNv = await context.NhanVien.FirstOrDefaultAsync(n => n.MaNhanVien == MaNhanVien);
                if (dbNv == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên cần cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dbNv.TenNhanVien = TenNhanVien;
                await context.SaveChangesAsync();
            }

            MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
