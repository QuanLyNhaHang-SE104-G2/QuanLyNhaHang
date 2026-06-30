using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class DonViTinhItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _maDonViTinh = "";

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private bool _isReadOnly = true;

    [ObservableProperty]
    private bool _isNew;
}
