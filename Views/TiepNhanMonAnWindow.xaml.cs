using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Views;

public partial class TiepNhanMonAnWindow : Window
{
    public TiepNhanMonAnWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<TiepNhanMonAnViewModel>();
    }
}
