using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class LoaiMonAnItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maLoaiMonAn = "";

    [ObservableProperty]
    private string _tenLoaiMonAn = "";

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}
