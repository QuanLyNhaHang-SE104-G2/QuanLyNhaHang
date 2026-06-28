using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SoDoBanViewModel _soDoBanViewModel;
    private readonly OrderViewModel _orderViewModel;
    private readonly MonAnViewModel _monAnViewModel;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private bool _isSoDoBanActive = true;

    [ObservableProperty]
    private bool _isThucDonActive;

    [ObservableProperty]
    private bool _isPosActive;

    public MainViewModel(SoDoBanViewModel soDoBanViewModel, OrderViewModel orderViewModel, MonAnViewModel monAnViewModel)
    {
        _soDoBanViewModel = soDoBanViewModel;
        _orderViewModel = orderViewModel;
        _monAnViewModel = monAnViewModel;
        CurrentViewModel = _soDoBanViewModel;
    }

    [RelayCommand]
    private void NavigateToSoDoBan()
    {
        CurrentViewModel = _soDoBanViewModel;
        IsSoDoBanActive = true;
        IsThucDonActive = false;
        IsPosActive = false;
        _soDoBanViewModel.LoadDataAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private void NavigateToPos()
    {
        CurrentViewModel = _orderViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = false;
        IsPosActive = true;
        _orderViewModel.LoadDataAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private void NavigateToThucDon()
    {
        CurrentViewModel = _monAnViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = true;
        IsPosActive = false;
        _monAnViewModel.LoadDataAsync().SafeFireAndForget();
    }
}
