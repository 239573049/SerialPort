using BlazorComponent;
using SerialPort.Desktop.Modules;

namespace SerialPort.Desktop.Pages;

public partial class Home
{
    private List<SerialPortDto> _serialPortDtos = new()
    {
        new SerialPortDto("调试内容"),
    };

    private List<BaudRateDto> _baudRateDtos = new()
    {
        new(1382400),
        new(921600),
        new(460800),
        new(256000),
        new(230400),
        new(128000),
        new(115200),
        new(76800),
        new(57600),
        new(43000),
        new(38400),
        new(19200),
        new(14400),
        new(9600),
        new(4800),
        new(2400),
        new(1200)
    };

    private List<StopBitDto> _stopBitDtos = new()
    {
        new (1),
        new (1.5),
        new (2),
    };

    private List<DataBitDto> _dataBitDtos = new List<DataBitDto>()
    {
        new (8),
        new (7),
        new (6),
        new (5),
    };

    private List<CheckBitDto> _checkBitDtos = new()
    {
        new (CheckBitType.None),
        new (CheckBitType.Odd),
        new (CheckBitType.Even),
    };

    private Guid _serialPortId;

    private int _baudRateId = 9600;
    
    private double _stopBitId = 1;
    
    private int _dataBitId = 8;
    
    private CheckBitType _checkBitId = CheckBitType.None;

    private StringNumber _tab;
    
    private string OpenSerialPortText = "打开串口";
    private async Task<object> InitEditor()
    {
        object options = new
        {
            language = "csharp",
            theme = "vs-dark",
            automaticLayout = true,
        };

        await Task.CompletedTask;

        return options;
    }

    private async Task OpenSerialPort()
    {
        OpenSerialPortText = "正在打开串口";
        
        await Task.Delay(1000);
        
        OpenSerialPortText = "关闭串口";
    }
}