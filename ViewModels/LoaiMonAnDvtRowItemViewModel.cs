using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class LoaiMonAnDvtRowItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _maLoaiMonAn = "";

    [ObservableProperty]
    private string _tenLoaiMonAn = "";

    [ObservableProperty]
    private ObservableCollection<LoaiMonAnDvtCellItemViewModel> _cells = [];
}
