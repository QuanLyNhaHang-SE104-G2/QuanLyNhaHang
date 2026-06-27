using System.Windows;

namespace QuanLyNhaHang.Services;

public interface IDialogService
{
    bool? ShowTiepNhanBanAnDialog(Window? owner);
    bool? ShowTiepNhanMonAnDialog(Window? owner);
}
