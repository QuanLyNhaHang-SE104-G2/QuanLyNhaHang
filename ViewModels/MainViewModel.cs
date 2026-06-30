using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SoDoBanViewModel _soDoBanViewModel;
    private readonly OrderViewModel _orderViewModel;
    private readonly MonAnViewModel _monAnViewModel;
    private readonly HoaDonViewModel _hoaDonViewModel;
    private readonly BaoCaoViewModel _baoCaoViewModel;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private bool _isSoDoBanActive = true;

    [ObservableProperty]
    private bool _isThucDonActive;

    [ObservableProperty]
    private bool _isPosActive;

    [ObservableProperty]
    private bool _isBaoCaoActive;

    [ObservableProperty]
    private bool _isHoaDonActive;

    public MainViewModel(
        SoDoBanViewModel soDoBanViewModel, 
        OrderViewModel orderViewModel, 
        MonAnViewModel monAnViewModel,
        HoaDonViewModel hoaDonViewModel,
        BaoCaoViewModel baoCaoViewModel)
    {
        _soDoBanViewModel = soDoBanViewModel;
        _orderViewModel = orderViewModel;
        _monAnViewModel = monAnViewModel;
        _hoaDonViewModel = hoaDonViewModel;
        _baoCaoViewModel = baoCaoViewModel;
        CurrentViewModel = _soDoBanViewModel;
    }

    [RelayCommand]
    private void NavigateToSoDoBan()
    {
        CurrentViewModel = _soDoBanViewModel;
        IsSoDoBanActive = true;
        IsThucDonActive = false;
        IsPosActive = false;
        IsBaoCaoActive = false;
        IsHoaDonActive = false;
        _soDoBanViewModel.LoadDataAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private void NavigateToPos()
    {
        CurrentViewModel = _orderViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = false;
        IsPosActive = true;
        IsBaoCaoActive = false;
        IsHoaDonActive = false;
        _orderViewModel.LoadDataAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private void NavigateToThucDon()
    {
        CurrentViewModel = _monAnViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = true;
        IsPosActive = false;
        IsBaoCaoActive = false;
        IsHoaDonActive = false;
        _monAnViewModel.LoadDataAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private void NavigateToBaoCao()
    {
        CurrentViewModel = _baoCaoViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = false;
        IsPosActive = false;
        IsBaoCaoActive = true;
        IsHoaDonActive = false;
        _baoCaoViewModel.LoadAllData();
    }

    [RelayCommand]
    private void NavigateToHoaDon()
    {
        CurrentViewModel = _hoaDonViewModel;
        IsSoDoBanActive = false;
        IsThucDonActive = false;
        IsPosActive = false;
        IsBaoCaoActive = false;
        IsHoaDonActive = true;
        _hoaDonViewModel.LoadDataAsync().SafeFireAndForget();
    }
}
