using System.Windows;

namespace QuanLyNhaHang.Services;

public interface IDialogService
{
    bool? ShowTiepNhanBanAnDialog(Window? owner);
    bool? ShowTiepNhanMonAnDialog(Window? owner);
    void ShowTraCuuBanAnDialog(Window? owner);
    bool? ShowTiepNhanPhieuGoiMonDialog(Window? owner);
    void ShowTraCuuPhieuGoiMonDialog(Window? owner);
}
