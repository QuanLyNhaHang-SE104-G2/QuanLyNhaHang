using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public abstract partial class PaginatedViewModelBase : ObservableObject, IDisposable
{
    private bool _disposed;

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
    protected virtual string EntityLabel => "mục";

    private CancellationTokenSource? _cts;

    public async Task LoadDataAsync()
    {
        if (_disposed) return;

        var newCts = new CancellationTokenSource();
        var oldCts = Interlocked.Exchange(ref _cts, newCts);

        // Always cancel the displaced token immediately to abort the prior operation
        if (oldCts != null)
        {
            try { oldCts.Cancel(); } catch (ObjectDisposedException) { }
            oldCts.Dispose();
        }

        // Evaluate if disposal occurred during the token exchange step
        if (_disposed)
        {
            try { newCts.Cancel(); } catch (ObjectDisposedException) { }
            newCts.Dispose();

            // Safely clear _cts if it still points to our local newCts instance
            Interlocked.CompareExchange(ref _cts, null, newCts);
            return;
        }

        var token = newCts.Token;
        try
        {
            await OnLoadDataAsync(token);
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }

    protected abstract Task OnLoadDataAsync(CancellationToken cancellationToken);

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
            LoadDataAsync().SafeFireAndForget();
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
            LoadDataAsync().SafeFireAndForget();
        }
    }

    public virtual void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        var cts = Interlocked.Exchange(ref _cts, null);
        try
        {
            cts?.Cancel();
        }
        catch (ObjectDisposedException) { }
        cts?.Dispose();
    }
}
