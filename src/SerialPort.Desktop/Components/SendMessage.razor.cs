using Microsoft.AspNetCore.Components;

namespace SerialPort.Desktop.Components;

public partial class SendMessage
{
    private string value;

    [Parameter]
    public bool SendDisabled { get; set; }

    /// <summary>
    /// 发送事件
    /// </summary>
    [Parameter]
    public EventCallback<string> OnSendMessage { get; set; }

    private async Task Send()
    {
        await OnSendMessage.InvokeAsync(value);
        value = string.Empty;
    }
}