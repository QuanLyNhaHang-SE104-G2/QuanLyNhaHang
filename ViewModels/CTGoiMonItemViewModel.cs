using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.ViewModels;

public partial class CTGoiMonItemViewModel : ObservableValidator
{
    private readonly List<MonAn> _dishes;

    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private int? _selectedMaMonAn;

    [ObservableProperty]
    [CustomValidation(typeof(CTGoiMonItemViewModel), nameof(ValidateSoLuong))]
    private string _soLuong = "1";

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private long _donGia;

    [ObservableProperty]
    private string _donGiaText = "";

    [ObservableProperty]
    private string _ghiChu = "";

    public event Action? OnItemChanged;

    public CTGoiMonItemViewModel(List<MonAn> dishes)
    {
        _dishes = dishes;
    }

    partial void OnSelectedMaMonAnChanged(int? value)
    {
        if (value.HasValue)
        {
            var dish = _dishes.FirstOrDefault(m => m.MaMonAn == value.Value);
            if (dish != null)
            {
                TenDonViTinh = dish.DonViTinh?.TenDonViTinh ?? "";
                DonGia = dish.DonGia;
                DonGiaText = $"{dish.DonGia:N0} VND";
            }
            else
            {
                TenDonViTinh = "";
                DonGia = 0;
                DonGiaText = "";
            }
        }
        else
        {
            TenDonViTinh = "";
            DonGia = 0;
            DonGiaText = "";
        }
        OnItemChanged?.Invoke();
    }

    partial void OnSoLuongChanged(string value)
    {
        ValidateProperty(value, nameof(SoLuong));
        OnItemChanged?.Invoke();
    }

    public int GetSoLuongParsed()
    {
        if (int.TryParse(SoLuong, out int val) && val > 0)
        {
            return val;
        }
        return 0;
    }

    public void ValidateRow()
    {
        ValidateAllProperties();
    }

    public static ValidationResult? ValidateSoLuong(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int val) || val <= 0)
        {
            return new ValidationResult("Số lượng phải là số nguyên dương.");
        }
        return ValidationResult.Success;
    }
}
