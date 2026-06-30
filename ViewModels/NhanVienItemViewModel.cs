using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class NhanVienItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private int _maNhanVien;

    [ObservableProperty]
    private string _tenNhanVien = "";
}
