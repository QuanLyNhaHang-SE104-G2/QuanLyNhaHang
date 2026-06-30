using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaHang.Extensions;

namespace QuanLyNhaHang.ViewModels;

public abstract partial class EditablePaginatedViewModelBase : ObservableValidator, System.IDisposable
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
    private System.Collections.ObjectModel.ObservableCollection<int> _pageNumbers = [];

    public int[] PageSizes { get; } = [10, 25, 50, 100];
    protected virtual string EntityLabel => "mục";

    private CancellationTokenSource? _cts;

    public async Task LoadDataAsync()
    {
        if (_disposed) return;

        var newCts = new CancellationTokenSource();
        var oldCts = Interlocked.Exchange(ref _cts, newCts);

        if (oldCts != null)
        {
            try { oldCts.Cancel(); } catch (System.ObjectDisposedException) { }
            oldCts.Dispose();
        }

        if (_disposed)
        {
            try { newCts.Cancel(); } catch (System.ObjectDisposedException) { }
            newCts.Dispose();
            Interlocked.CompareExchange(ref _cts, null, newCts);
            return;
        }

        var token = newCts.Token;
        try
        {
            await OnLoadDataAsync(token);
        }
        catch (System.OperationCanceledException) { }
        catch (System.ObjectDisposedException) { }
    }

    protected abstract Task OnLoadDataAsync(CancellationToken cancellationToken);

    [RelayCommand]
    private void PrevPage()
    {
        if (PageNumber > 1)
        {
            PageNumber--;
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        int totalPages = (int)System.Math.Ceiling((double)TotalItems / PageSize);
        if (totalPages < 1) totalPages = 1;
        if (PageNumber < totalPages)
        {
            PageNumber++;
        }
    }

    protected void UpdatePaginationInfo()
    {
        int totalPages = (int)System.Math.Ceiling((double)TotalItems / PageSize);
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
        int end = System.Math.Min(PageNumber * PageSize, TotalItems);
        TotalInfoText = $"Hiển thị {start}-{end} trên tổng số {TotalItems} {EntityLabel}";
    }

    partial void OnPageSizeChanged(int value)
    {
        if (PageNumber == 1)
        {
            OnPageChanged();
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
            OnPageChanged();
        }
    }

    protected abstract void OnPageChanged();

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
        catch (System.ObjectDisposedException) { }
        cts?.Dispose();
    }
}
