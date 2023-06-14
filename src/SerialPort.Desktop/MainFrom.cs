using Masa.Blazor;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace SerialPort.Desktop;

public partial class MainFrom : Form
{
    public MainFrom()
    {
        InitializeComponent();

        MinimumSize = new Size(850, 700);
        
        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        services.AddMasaBlazor(options =>
        {
            options.ConfigureTheme(theme =>
            {
                theme.Dark = false;
            });
        });
        services.AddBlazorWebViewDeveloperTools();
        blazorWebView1.HostPage = "wwwroot/index.html";
        blazorWebView1.Services = services.BuildServiceProvider();
        blazorWebView1.RootComponents.Add<App>("#app");
    }
}