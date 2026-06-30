using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class LoaiBanItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maLoaiBan = "";

    [ObservableProperty]
    private string _tenLoaiBan = "";

    [ObservableProperty]
    private long _phuThu;

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}
