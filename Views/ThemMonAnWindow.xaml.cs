using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Views;

public partial class ThemMonAnWindow : Window
{
    public ThemMonAnWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<ThemMonAnViewModel>();
    }
}
