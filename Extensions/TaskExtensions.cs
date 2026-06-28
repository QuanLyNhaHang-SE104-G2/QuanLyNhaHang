using System.Diagnostics;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Safely executes a fire-and-forget task, observing exceptions so they
    /// do not propagate to <see cref="TaskScheduler.UnobservedTaskException"/>
    /// and potentially crash the application.
    /// Cancellation and object-disposal exceptions are silently swallowed.
    /// All other exceptions are forwarded to <paramref name="onError"/> (if
    /// provided) and logged to the debug output.
    /// </summary>
    public static async void SafeFireAndForget(
        this Task task,
        System.Action<System.Exception>? onError = null)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException) { }
        catch (System.ObjectDisposedException) { }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"[SafeFireAndForget] Unobserved exception: {ex}");
            onError?.Invoke(ex);
        }
    }
}
