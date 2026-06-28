using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class OrderItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private int _maPhieuGoiMon;

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _thoiGianGoiText = "";

    [ObservableProperty]
    private string _tenNhanVien = "";

    [ObservableProperty]
    private string _tenTrangThai = "";

    [ObservableProperty]
    private string _maTrangThai = "";

    [ObservableProperty]
    private string _tongTienText = "";
}
