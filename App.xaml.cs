using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Services;
using QuanLyNhaHang.ViewModels;

namespace QuanLyNhaHang;

/// <summary>
/// Main application shell
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current = (App)Application.Current;
    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }
    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<SoDoBanViewModel>();
        services.AddTransient<TiepNhanBanAnViewModel>();
        return services.BuildServiceProvider();
    }

    public App()
    {
        Services = ConfigureServices();

        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }
    }
}