using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLyNhaHang.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SoDoBanViewModel _soDoBanViewModel;
    private readonly MonAnViewModel _monAnViewModel;
    private readonly OrderViewModel _orderViewModel;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private bool _isSoDoBanActive = true;

    [ObservableProperty]
    private bool _isThucDonActive;

    [ObservableProperty]
    private bool _isPosActive;

    public MainViewModel(SoDoBanViewModel soDoBanViewModel, MonAnViewModel monAnViewModel, OrderViewModel orderViewModel)
    {
        _soDoBanViewModel = soDoBanViewModel;
        _monAnViewModel = monAnViewModel;
        _orderViewModel = orderViewModel;
        CurrentViewModel = _soDoBanViewModel;
    }

    [RelayCommand]
    private void NavigateToSoDoBan()
    {
        CurrentViewModel = _soDoBanViewModel;
        IsSoDoBanActive = true;
        IsThucDonActive = false;
        IsPosActive = false;
        _ = _soDoBanViewModel.LoadDataAsync();
    }

    [RelayCommand]
    private void NavigateToThucDon()
    {
        CurrentViewModel = _monAnViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = true;
        IsPosActive = false;
        _ = _monAnViewModel.LoadDataAsync();
    }

    [RelayCommand]
    private void NavigateToPos()
    {
        CurrentViewModel = _orderViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = false;
        IsPosActive = true;
        _ = _orderViewModel.LoadDataAsync();
    }
}
