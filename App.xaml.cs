using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Services;
using QuanLyNhaHang.ViewModels;
using QuanLyNhaHang.Views;

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
        string connString = AppDbContext.LoadConnectionStringFromConfig() ?? "Data Source=QuanLyNhaHang.db";
        services.AddDbContextFactory<AppDbContext>(
            options => options
                .UseSqlite(connString));

        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<SoDoBanViewModel>();
        services.AddTransient<TiepNhanBanAnViewModel>();
        services.AddTransient<MonAnViewModel>();
        services.AddTransient<TiepNhanMonAnViewModel>();
        services.AddTransient<TraCuuBanAnViewModel>();
        services.AddTransient<MainViewModel>();
        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            }
        );
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
            context.Database.Migrate();
        }
    }
}