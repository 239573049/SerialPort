using Microsoft.AspNetCore.Components;

namespace SerialPort.Desktop.Components;

public partial class SendMessage
{
    [Parameter]
    public bool SendDisabled { get; set; }
}