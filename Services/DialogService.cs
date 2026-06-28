using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        var viewModel = App.Current.Services.GetRequiredService<TiepNhanBanAnViewModel>();
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
    {
        var dialog = new TiepNhanMonAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TiepNhanMonAnViewModel>();
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
    {
        var dialog = new TraCuuBanAnWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TraCuuBanAnViewModel>();
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

    public bool? ShowTiepNhanPhieuGoiMonDialog(Window? owner)
    {
        var dialog = new TiepNhanPhieuGoiMonWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TiepNhanPhieuGoiMonViewModel>();
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
    {
        var dialog = new TraCuuPhieuGoiMonWindow
        {
            Owner = owner ?? Application.Current.MainWindow
        };
        var viewModel = App.Current.Services.GetRequiredService<TraCuuPhieuGoiMonViewModel>();
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
}
