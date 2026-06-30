using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhViewModel : ObservableObject
{
    public QuyDinhBanAnViewModel QuyDinhBanAn { get; }
    public QuyDinhLoaiMonAnViewModel QuyDinhLoaiMonAn { get; }
    public QuyDinhDonViTinhViewModel QuyDinhDonViTinh { get; }
    public QuyDinhTrangThaiThanhToanViewModel QuyDinhTrangThaiThanhToan { get; }

    public QuyDinhViewModel(
        QuyDinhBanAnViewModel quyDinhBanAn,
        QuyDinhLoaiMonAnViewModel quyDinhLoaiMonAn,
        QuyDinhDonViTinhViewModel quyDinhDonViTinh,
        QuyDinhTrangThaiThanhToanViewModel quyDinhTrangThaiThanhToan)
    {
        QuyDinhBanAn = quyDinhBanAn;
        QuyDinhLoaiMonAn = quyDinhLoaiMonAn;
        QuyDinhDonViTinh = quyDinhDonViTinh;
        QuyDinhTrangThaiThanhToan = quyDinhTrangThaiThanhToan;
    }

    public void LoadAllData()
    {
        QuyDinhBanAn.InitializeFormAsync().SafeFireAndForget();
        QuyDinhLoaiMonAn.InitializeFormAsync().SafeFireAndForget();
        QuyDinhDonViTinh.InitializeFormAsync().SafeFireAndForget();
        QuyDinhTrangThaiThanhToan.InitializeFormAsync().SafeFireAndForget();
    }
}
