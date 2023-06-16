using Microsoft.JSInterop;

namespace SerialPort.Desktop.Components;

public partial class Help
{
    private const string GitHub = "https://github.com/239573049/SerialPort";

    private async Task GoTo(string url)
    {
        await JsRuntime.InvokeVoidAsync("open", url);
    }
}
