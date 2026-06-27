using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLyNhaHang.ViewModels;

public abstract partial class PaginatedViewModelBase : ObservableObject
{
    [ObservableProperty]
    private int _pageSize = 10;

    [ObservableProperty]
    private int _pageNumber = 1;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private string _totalInfoText = "";

    [ObservableProperty]
    private ObservableCollection<int> _pageNumbers = [];

    public int[] PageSizes { get; } = [10, 25, 50, 100];

    private CancellationTokenSource? _cts;

    public async Task LoadDataAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            await OnLoadDataAsync(token);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
    }

    protected abstract Task OnLoadDataAsync(CancellationToken cancellationToken);

    protected virtual string EntityLabel => "mục";

    protected void UpdatePaginationInfo()
    {
        int totalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
        if (totalPages < 1) totalPages = 1;

        int currentCount = PageNumbers.Count;
        if (currentCount < totalPages)
        {
            for (int i = currentCount + 1; i <= totalPages; i++)
            {
                PageNumbers.Add(i);
            }
        }
        else if (currentCount > totalPages)
        {
            for (int i = currentCount - 1; i >= totalPages; i--)
            {
                PageNumbers.RemoveAt(i);
            }
        }

        if (PageNumber < 1)
        {
            PageNumber = 1;
        }
        else if (PageNumber > totalPages)
        {
            PageNumber = totalPages;
        }

        int start = TotalItems == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
        int end = Math.Min(PageNumber * PageSize, TotalItems);
        TotalInfoText = $"Hiển thị {start}-{end} trên tổng số {TotalItems} {EntityLabel}";
    }

    [RelayCommand]
    private async Task PrevPageAsync()
    {
        if (PageNumber > 1)
        {
            PageNumber--;
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        int totalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
        if (totalPages < 1) totalPages = 1;
        if (PageNumber < totalPages)
        {
            PageNumber++;
            await LoadDataAsync();
        }
    }

    partial void OnPageSizeChanged(int value)
    {
        if (PageNumber == 1)
        {
            _ = LoadDataAsync();
        }
        else
        {
            PageNumber = 1;
        }
    }

    partial void OnPageNumberChanged(int value)
    {
        if (value >= 1)
        {
            _ = LoadDataAsync();
        }
    }
}
