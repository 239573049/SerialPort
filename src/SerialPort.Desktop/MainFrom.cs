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

        // Ĭ����С�����С
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

    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        switch (extension)
        {
            case ".html":
                return "text/html";
            case ".css":
                return "text/css";
            case ".js":
                return "text/javascript";
            case ".png":
                return "image/png";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".gif":
                return "image/gif";
            case ".ico":
                return "image/x-icon";
            default:
                return "application/octet-stream";
        }
    }
}