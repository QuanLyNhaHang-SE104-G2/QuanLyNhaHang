using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanMonAnItemViewModel : ObservableValidator
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    [Required(ErrorMessage = "Tên món ăn không được để trống.")]
    private string _tenMonAn = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn đơn vị tính.")]
    private string _selectedMaDonViTinh = "";

    [ObservableProperty]
    [Required(ErrorMessage = "Vui lòng chọn tình trạng.")]
    private string _selectedMaTinhTrang = "";

    [ObservableProperty]
    [CustomValidation(typeof(TiepNhanMonAnItemViewModel), nameof(ValidateDonGia))]
    private string _donGia = "";

    partial void OnTenMonAnChanged(string value) => ValidateProperty(value, nameof(TenMonAn));
    partial void OnSelectedMaDonViTinhChanged(string value) => ValidateProperty(value, nameof(SelectedMaDonViTinh));
    partial void OnSelectedMaTinhTrangChanged(string value) => ValidateProperty(value, nameof(SelectedMaTinhTrang));
    partial void OnDonGiaChanged(string value) => ValidateProperty(value, nameof(DonGia));

    public void ValidateRow()
    {
        ValidateAllProperties();
    }

    public static ValidationResult? ValidateDonGia(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value) || !long.TryParse(value, out long donGia) || donGia < 0)
        {
            return new ValidationResult("Đơn giá phải là số nguyên hợp lệ và lớn hơn hoặc bằng 0.");
        }
        return ValidationResult.Success;
    }
}
