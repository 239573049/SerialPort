using Masa.Blazor;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using SerialPort.Desktop.JSInterp;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace SerialPort.Desktop;

public partial class MainFrom : Form
{
    public MainFrom()
    {
        InitializeComponent();

        // 默认最小窗体大小
        MinimumSize = new Size(850, 750);

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