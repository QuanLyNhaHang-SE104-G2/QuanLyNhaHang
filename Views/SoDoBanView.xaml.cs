using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang.Views;

/// <summary>
/// Interaction logic for SoDoBanView.xaml
/// </summary>
public partial class SoDoBanView : UserControl
{
    public SoDoBanView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<SoDoBanViewModel>();
    }
}
