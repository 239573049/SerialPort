using BlazorComponent.JSInterop;
using Microsoft.JSInterop;

namespace SerialPort.Desktop.JSInterp;

internal class SerialJSInterp : JSModule
{
    public SerialJSInterp(IJSRuntime js) : base(js, "/js/serial.js")
    {
    }

    public async ValueTask Init()
    {
        await InvokeVoidAsync("init");
    }
}
