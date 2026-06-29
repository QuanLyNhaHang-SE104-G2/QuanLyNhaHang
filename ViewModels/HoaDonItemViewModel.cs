using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class HoaDonItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private int _maHoaDon;

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _thoiGianThanhToanText = "";

    [ObservableProperty]
    private string _phuThuText = "";

    [ObservableProperty]
    private string _tongTienText = "";
}
