using System.Linq;
using System.Windows;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHang.Views;

namespace QuanLyNhaHang.Services;

public class DialogService : IDialogService
{
    public bool? ShowTiepNhanBanAnDialog()
    {
        var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) 
                           ?? Application.Current.MainWindow;
        var dialog = new TiepNhanBanAnWindow
        {
            Owner = activeWindow
        };
        return dialog.ShowDialog();
    }

    public bool? ShowTiepNhanMonAnDialog()
    {
        var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) 
                           ?? Application.Current.MainWindow;
        var dialog = new TiepNhanMonAnWindow
        {
            Owner = activeWindow
        };
        return dialog.ShowDialog();
    }
}
