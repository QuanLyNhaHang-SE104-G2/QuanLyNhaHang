using System.Linq;
using System.Windows;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHang.Views;

namespace QuanLyNhaHang.Services;

public class DialogService : IDialogService
{
    public bool? ShowTiepNhanBanAnDialog(Window? owner)
    {
        var dialog = new TiepNhanBanAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        return dialog.ShowDialog();
    }

    public bool? ShowTiepNhanMonAnDialog(Window? owner)
    {
        var dialog = new TiepNhanMonAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        return dialog.ShowDialog();
    }
}
