using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class MonAnDisplayViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maMonAn = "";

    [ObservableProperty]
    private string _tenMonAn = "";

    [ObservableProperty]
    private string _tenLoaiMonAn = "";

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private string _donGiaText = "";

    [ObservableProperty]
    private string _tenTinhTrang = "";
}
