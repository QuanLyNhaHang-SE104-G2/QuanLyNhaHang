using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class BaoCaoViewModel : ObservableObject
{
    public BaoCaoDoanhThuTheoNgayViewModel DoanhThuTheoNgay { get; }
    public BaoCaoDoanhThuTheoMonAnViewModel DoanhThuTheoMonAn { get; }

    public BaoCaoViewModel(
        BaoCaoDoanhThuTheoNgayViewModel doanhThuTheoNgay,
        BaoCaoDoanhThuTheoMonAnViewModel doanhThuTheoMonAn)
    {
        DoanhThuTheoNgay = doanhThuTheoNgay;
        DoanhThuTheoMonAn = doanhThuTheoMonAn;
    }

    public void LoadAllData()
    {
        DoanhThuTheoNgay.LoadDataAsync().SafeFireAndForget();
        DoanhThuTheoMonAn.LoadDataAsync().SafeFireAndForget();
    }
}
