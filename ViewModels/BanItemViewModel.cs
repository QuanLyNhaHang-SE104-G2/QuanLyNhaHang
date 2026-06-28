using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class BanItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private int _maBan;

    [ObservableProperty]
    private string _tenBan = "";

    [ObservableProperty]
    private string _khuVuc = "";

    [ObservableProperty]
    private int _soChoNgoi;

    [ObservableProperty]
    private string _soChoNgoiText = "";

    [ObservableProperty]
    private string _tenLoaiBan = "";

    [ObservableProperty]
    private string _phuThuText = "";
}
