using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Views;

/// <summary>
/// Interaction logic for TiepNhanBanAnWindow.xaml
/// </summary>
public partial class TiepNhanBanAnWindow : Window
{
    public TiepNhanBanAnWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<TiepNhanBanAnViewModel>();
    }
}
