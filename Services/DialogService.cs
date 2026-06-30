using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHang.Views;

namespace QuanLyNhaHang.Services;

public class DialogService : IDialogService
{
    private static bool? ShowDialog<TWindow, TViewModel>(Window? owner)
        where TWindow : Window, new()
        where TViewModel : class
    {
        var dialog = new TWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TViewModel>();
        dialog.DataContext = viewModel;
        try
        {
            return dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    private static void ShowDialogNoResult<TWindow, TViewModel>(Window? owner)
        where TWindow : Window, new()
        where TViewModel : class
    {
        var dialog = new TWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TViewModel>();
        dialog.DataContext = viewModel;
        try
        {
            dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    public bool? ShowTiepNhanBanAnDialog(Window? owner)
        => ShowDialog<TiepNhanBanAnWindow, TiepNhanBanAnViewModel>(owner);

    public bool? ShowCapNhatBanAnDialog(Window? owner, int maBan)
    {
        var dialog = new CapNhatBanAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<CapNhatBanAnViewModel>();
        viewModel.MaBan = maBan;
        dialog.DataContext = viewModel;

        try
        {
            return dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    public bool? ShowTiepNhanMonAnDialog(Window? owner)
        => ShowDialog<TiepNhanMonAnWindow, TiepNhanMonAnViewModel>(owner);

    public bool? ShowCapNhatMonAnDialog(Window? owner, int maMonAn)
    {
        var dialog = new CapNhatMonAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<CapNhatMonAnViewModel>();
        viewModel.MaMonAn = maMonAn;
        dialog.DataContext = viewModel;

        try
        {
            return dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    public void ShowTraCuuBanAnDialog(Window? owner)
        => ShowDialogNoResult<TraCuuBanAnWindow, TraCuuBanAnViewModel>(owner);

    public void ShowTraCuuMonAnDialog(Window? owner)
        => ShowDialogNoResult<TraCuuMonAnWindow, TraCuuMonAnViewModel>(owner);

    public bool? ShowTiepNhanPhieuGoiMonDialog(Window? owner)
        => ShowDialog<TiepNhanPhieuGoiMonWindow, TiepNhanPhieuGoiMonViewModel>(owner);

    public bool? ShowCapNhatPhieuGoiMonDialog(Window? owner, int maPhieuGoiMon)
    {
        var dialog = new CapNhatPhieuGoiMonWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<CapNhatPhieuGoiMonViewModel>();
        viewModel.MaPhieuGoiMon = maPhieuGoiMon;
        dialog.DataContext = viewModel;

        try
        {
            return dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    public bool? ShowTiepNhanNhanVienDialog(Window? owner)
        => ShowDialog<TiepNhanNhanVienWindow, TiepNhanNhanVienViewModel>(owner);

    public bool? ShowCapNhatNhanVienDialog(Window? owner, int maNhanVien)
    {
        var dialog = new CapNhatNhanVienWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<CapNhatNhanVienViewModel>();
        viewModel.MaNhanVien = maNhanVien;
        dialog.DataContext = viewModel;

        try
        {
            return dialog.ShowDialog();
        }
        finally
        {
            if (viewModel is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }
    }

    public void ShowTraCuuPhieuGoiMonDialog(Window? owner)
        => ShowDialogNoResult<TraCuuPhieuGoiMonWindow, TraCuuPhieuGoiMonViewModel>(owner);

    public bool? ShowTiepNhanHoaDonDialog(Window? owner)
        => ShowDialog<TiepNhanHoaDonWindow, TiepNhanHoaDonViewModel>(owner);

    public void ShowTraCuuHoaDonDialog(Window? owner)
        => ShowDialogNoResult<TraCuuHoaDonWindow, TraCuuHoaDonViewModel>(owner);
}
