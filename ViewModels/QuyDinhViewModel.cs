using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class QuyDinhViewModel : ObservableObject
{
    public QuyDinh1ViewModel QuyDinh1 { get; }
    public QuyDinh3ViewModel QuyDinh3 { get; }
    public QuyDinh5ViewModel QuyDinh5 { get; }

    public QuyDinhViewModel(
        QuyDinh1ViewModel quyDinh1,
        QuyDinh3ViewModel quyDinh3,
        QuyDinh5ViewModel quyDinh5)
    {
        QuyDinh1 = quyDinh1;
        QuyDinh3 = quyDinh3;
        QuyDinh5 = quyDinh5;
    }

    public void LoadAllData()
    {
        QuyDinh1.LoadDataAsync().SafeFireAndForget();
        QuyDinh3.LoadDataAsync().SafeFireAndForget();
        QuyDinh5.LoadDataAsync().SafeFireAndForget();
    }
}
