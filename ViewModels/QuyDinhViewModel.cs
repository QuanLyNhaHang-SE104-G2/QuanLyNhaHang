using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhViewModel : ObservableObject
{
    public QuyDinhBanAnViewModel QuyDinhBanAn { get; }
    public QuyDinhLoaiMonAnViewModel QuyDinhLoaiMonAn { get; }
    public QuyDinhDonViTinhViewModel QuyDinhDonViTinh { get; }
    public QuyDinhTrangThaiThanhToanViewModel QuyDinhTrangThaiThanhToan { get; }
    public QuyDinhLoaiMonAnDvtViewModel QuyDinhLoaiMonAnDvt { get; }

    public QuyDinhViewModel(
        QuyDinhBanAnViewModel quyDinhBanAn,
        QuyDinhLoaiMonAnViewModel quyDinhLoaiMonAn,
        QuyDinhDonViTinhViewModel quyDinhDonViTinh,
        QuyDinhTrangThaiThanhToanViewModel quyDinhTrangThaiThanhToan,
        QuyDinhLoaiMonAnDvtViewModel quyDinhLoaiMonAnDvt)
    {
        QuyDinhBanAn = quyDinhBanAn;
        QuyDinhLoaiMonAn = quyDinhLoaiMonAn;
        QuyDinhDonViTinh = quyDinhDonViTinh;
        QuyDinhTrangThaiThanhToan = quyDinhTrangThaiThanhToan;
        QuyDinhLoaiMonAnDvt = quyDinhLoaiMonAnDvt;
    }

    public void LoadAllData()
    {
        QuyDinhBanAn.InitializeFormAsync().SafeFireAndForget();
        QuyDinhLoaiMonAn.InitializeFormAsync().SafeFireAndForget();
        QuyDinhDonViTinh.InitializeFormAsync().SafeFireAndForget();
        QuyDinhTrangThaiThanhToan.InitializeFormAsync().SafeFireAndForget();
        QuyDinhLoaiMonAnDvt.InitializeFormAsync().SafeFireAndForget();
    }
}
