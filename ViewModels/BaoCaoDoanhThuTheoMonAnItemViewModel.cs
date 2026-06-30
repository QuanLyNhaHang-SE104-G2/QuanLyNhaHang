using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class BaoCaoDoanhThuTheoMonAnItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _tenMonAn = "";

    [ObservableProperty]
    private long _doanhThu;

    [ObservableProperty]
    private string _doanhThuText = "";

    [ObservableProperty]
    private double _tyLe;

    [ObservableProperty]
    private string _tyLeText = "";
}
