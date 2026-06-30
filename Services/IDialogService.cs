using System.Windows;

namespace QuanLyNhaHang.Services;

public interface IDialogService
{
    bool? ShowTiepNhanBanAnDialog(Window? owner);
    bool? ShowCapNhatBanAnDialog(Window? owner, int maBan);
    bool? ShowTiepNhanMonAnDialog(Window? owner);
    void ShowTraCuuBanAnDialog(Window? owner);
    bool? ShowTiepNhanPhieuGoiMonDialog(Window? owner);
    void ShowTraCuuPhieuGoiMonDialog(Window? owner);
    bool? ShowTiepNhanHoaDonDialog(Window? owner);
    void ShowTraCuuHoaDonDialog(Window? owner);
}
