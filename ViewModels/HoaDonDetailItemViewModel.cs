using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class HoaDonDetailItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _tenMonAn = "";

    [ObservableProperty]
    private int _soLuong;

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private long _donGia;

    [ObservableProperty]
    private string _donGiaText = "";

    [ObservableProperty]
    private string _thoiGianGoiText = "";

    [ObservableProperty]
    private long _thanhTien;

    [ObservableProperty]
    private string _thanhTienText = "";
}
