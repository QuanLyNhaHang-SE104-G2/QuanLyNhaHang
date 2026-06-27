using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLyNhaHang.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SoDoBanViewModel _soDoBanViewModel;
    private readonly MonAnViewModel _monAnViewModel;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private bool _isSoDoBanActive = true;

    [ObservableProperty]
    private bool _isThucDonActive;

    public MainViewModel(SoDoBanViewModel soDoBanViewModel, MonAnViewModel monAnViewModel)
    {
        _soDoBanViewModel = soDoBanViewModel;
        _monAnViewModel = monAnViewModel;
        CurrentViewModel = _soDoBanViewModel;
    }

    [RelayCommand]
    private void NavigateToSoDoBan()
    {
        CurrentViewModel = _soDoBanViewModel;
        IsSoDoBanActive = true;
        IsThucDonActive = false;
        _ = _soDoBanViewModel.LoadDataAsync();
    }

    [RelayCommand]
    private void NavigateToThucDon()
    {
        CurrentViewModel = _monAnViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = true;
        _ = _monAnViewModel.LoadDataAsync();
    }
}
