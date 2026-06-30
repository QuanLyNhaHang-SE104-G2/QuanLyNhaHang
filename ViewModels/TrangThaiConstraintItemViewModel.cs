using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class TrangThaiConstraintItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _maTrangThai = "";

    [ObservableProperty]
    private string _tenTrangThai = "";

    [ObservableProperty]
    private bool _duocThanhToan;
}
