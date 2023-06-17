using BlazorComponent;
using SerialPort.Desktop.Modules;
using System.IO.Ports;
using System.Text;

namespace SerialPort.Desktop.Pages;

public partial class Home : IAsyncDisposable
{
    private List<SerialPortDto> _serialPortDtos = new();

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
        new (StopBits.None),
        new (StopBits.One),
        new (StopBits.Two),
        new (StopBits.OnePointFive),
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

    private StopBits _stopBitId = StopBits.One;

    private int _dataBitId = 8;

    private CheckBitType _checkBitId = CheckBitType.None;

    private StringNumber _tab;

    private bool disposable;

    private System.IO.Ports.SerialPort serialPort;

    private string _debugMessage = string.Empty;

    /// <summary>
    /// 串口标签
    /// </summary>
    private string OpenSerialPortText = "打开串口";

    /// <summary>
    /// 发送间隔（ms）
    /// </summary>
    private int _sendInterval = 1000;

    /// <summary>
    /// 是否定时发送
    /// </summary>
    private bool _sendTime = false;

    private async Task<object> InitEditor()
    {
        object options = new
        {
            language = "serial",
            theme = "serialTheme",
            automaticLayout = true,
        };

        await Task.CompletedTask;

        return options;
    }

    private async Task OpenSerialPort()
    {
        var port = _serialPortDtos.First(x => x.Id == _serialPortId);

        if (port == null)
        {
            await PopupService.EnqueueSnackbarAsync(new SnackbarOptions()
            {
                Title = "请先选择串口",
                Type = AlertTypes.Error
            });

            return;
        }

        if (serialPort?.IsOpen == true)
        {
            serialPort.Close();
            OpenSerialPortText = "打开串口";
            return;
        }
        try
        {
            serialPort = new System.IO.Ports.SerialPort
            {
                PortName = port.Name,
                BaudRate = _baudRateId,
                StopBits = _stopBitId,
                DataBits = _dataBitId
            };

            serialPort.Open();

            serialPort.DataReceived += SerialPort_DataReceived;

            OpenSerialPortText = "关闭串口";
        }
        catch (UnauthorizedAccessException)
        {
            await PopupService.EnqueueSnackbarAsync(new SnackbarOptions()
            {
                Title = "打开串口失败",
                Type = AlertTypes.Error
            });
        }
    }

    private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var buffer = new byte[serialPort.ReadBufferSize];
        serialPort.Read(buffer, 0, buffer.Length);

        var message = Encoding.UTF8.GetString(buffer);

        _debugMessage += $"[接收] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";

        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Factory.StartNew(async () =>
        {
            if (disposable == false)
            {
                // 定时循环监听新串口
                await LoadSerialPortAsync();
                await InvokeAsync(StateHasChanged);
                await Task.Delay(10000);
            }
        });

        await SerialJSInterp.Init();
    }

    /// <summary>
    /// 定时加载串口
    /// </summary>
    /// <returns></returns>
    private async Task LoadSerialPortAsync()
    {
        string[] ports = System.IO.Ports.SerialPort.GetPortNames();

        _serialPortDtos.Clear();

        // 输出所有串口名称
        foreach (var port in ports)
        {
            _serialPortDtos.Add(new SerialPortDto(port));
        }
        await Task.CompletedTask;
    }

    private async Task OnSendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (serialPort == null || serialPort?.IsOpen == false)
        {
            await PopupService.EnqueueSnackbarAsync("串口未打开", AlertTypes.Error);
            return;
        }

        if (_sendTime)
        {
            try
            {

                while (_sendTime && serialPort!.IsOpen)
                {
                    serialPort!.WriteLine(message);
                    _debugMessage += $"[info] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(_sendInterval);
                }
            }
            catch (Exception e)
            {
                _debugMessage += $"[error] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";
            }
        }
        else
        {
            _debugMessage += $"[info] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";
            serialPort!.WriteLine(message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        disposable = true;
        await Task.CompletedTask;
    }
}