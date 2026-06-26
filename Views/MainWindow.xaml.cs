using System.Windows;

namespace QuanLyNhaHang.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Load the Sơ đồ bàn view as the default content panel
        MainContentControl.Content = new SoDoBanView();
    }
}
