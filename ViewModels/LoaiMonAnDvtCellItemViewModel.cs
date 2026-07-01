using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class LoaiMonAnDvtCellItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _maDonViTinh = "";

    [ObservableProperty]
    private string _tenDonViTinh = "";

    [ObservableProperty]
    private bool _isChecked;
}
