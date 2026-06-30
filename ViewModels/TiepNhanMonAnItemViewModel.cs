using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaHang.ViewModels;

public partial class TiepNhanMonAnItemViewModel : ObservableValidator
{
    [ObservableProperty]
    private int _sTT;

    [ObservableProperty]
    private string _tenMonAn = "";

    [ObservableProperty]
    private string _selectedMaDonViTinh = "";

    [ObservableProperty]
    private string _selectedMaTinhTrang = "";

    [ObservableProperty]
    private string _donGia = "";

    public void ValidateRow()
    {
        ValidateProperty(TenMonAn, nameof(TenMonAn));
        ValidateProperty(SelectedMaDonViTinh, nameof(SelectedMaDonViTinh));
        ValidateProperty(SelectedMaTinhTrang, nameof(SelectedMaTinhTrang));
        ValidateProperty(DonGia, nameof(DonGia));
    }
}
