using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using SerialPort.Desktop.JSInterp;

namespace SerialPort.Desktop;

public partial class MainFrom : Form
{
    public MainFrom()
    {
        InitializeComponent();

        // 默认最小窗体大小
        MinimumSize = new Size(850, 800);

        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        services.AddMasaBlazor(options =>
        {
            options.ConfigureTheme(theme =>
            {
                theme.Dark = false;
            });
        });
        services.AddScoped<SerialJSInterp>();
        services.AddBlazorWebViewDeveloperTools();

        blazorWebView1.HostPage = "wwwroot/index.html";
        blazorWebView1.Services = services.BuildServiceProvider();
        blazorWebView1.RootComponents.Add<App>("#app");

    }

}