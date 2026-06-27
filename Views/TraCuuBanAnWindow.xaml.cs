using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Views;

/// <summary>
/// Interaction logic for TraCuuBanAnWindow.xaml
/// </summary>
public partial class TraCuuBanAnWindow : Window
{
    public TraCuuBanAnWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<TraCuuBanAnViewModel>();
    }
}
