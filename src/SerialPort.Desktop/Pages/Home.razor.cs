using BlazorComponent;
using SerialPort.Desktop.Modules;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;
using SerialPort.Desktop.Components;

namespace SerialPort.Desktop.Pages;

public partial class Home : IAsyncDisposable
{
    
    [CascadingParameter(Name = nameof(SettingOptions))]
    public SettingOptions _settingOptions { get; set; }
    
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
        new(StopBits.None),
        new(StopBits.One),
        new(StopBits.Two),
        new(StopBits.OnePointFive),
    };

    private List<DataBitDto> _dataBitDtos = new()
    {
        new(8),
        new(7),
        new(6),
        new(5),
    };

    private List<CheckBitDto> _checkBitDtos = new()
    {
        new(CheckBitType.None),
        new(CheckBitType.Odd),
        new(CheckBitType.Even),
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

    /// <summary>
    /// 是否十六进制发送
    /// </summary>
    private bool HexSend = false;

    /// <summary>
    /// 是否十六进制显示
    /// </summary>
    private bool HexShow = false;

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
        var count = serialPort.Read(buffer, 0, buffer.Length);
        var message = string.Empty;

        buffer = buffer.Take(count).ToArray();

        // 转换hex
        message = HexShow ? string.Join(" ", buffer.Select(x => x.ToString("X2"))) : Encoding.UTF8.GetString(buffer);

        _debugMessage += $"[accept] {DateTime.Now:HH:mm:ss}：{message}{Environment.NewLine}";

        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Factory.StartNew(async () =>
        {
            if (disposable == false)
            {
                try
                {
                    // 定时循环监听新串口
                    await LoadSerialPortAsync();
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(10000);
                }
                catch (Exception)
                {
                }
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
                    if (HexSend)
                    {
                        try
                        {
                            message = message.Replace(" ", "");
                            byte[] hexArray = new byte[message.Length / 2];
                            for (int i = 0; i < message.Length; i += 2)
                            {
                                hexArray[i / 2] = Convert.ToByte(message.Substring(i, 2), 16);
                            }

                            serialPort.Write(hexArray, 0, hexArray.Length);
                            _debugMessage +=
                                $"[send] {DateTime.Now:HH:mm:ss}：{BitConverter.ToString(hexArray).Replace("-", "")}{Environment.NewLine}";
                        }
                        catch (Exception e)
                        {
                            await PopupService.EnqueueSnackbarAsync(new SnackbarOptions()
                            {
                                Content = e.Message,
                                Type = AlertTypes.Error
                            });
                            return;
                        }
                    }
                    else
                    {
                        serialPort!.WriteLine(message);
                        _debugMessage += $"[send] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";
                    }

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
            if (HexSend)
            {
                try
                {
                    message = message.Replace(" ", "");
                    byte[] hexArray = new byte[message.Length / 2];
                    for (int i = 0; i < message.Length; i += 2)
                    {
                        hexArray[i / 2] = Convert.ToByte(message.Substring(i, 2), 16);
                    }

                    serialPort.Write(hexArray, 0, hexArray.Length);
                    _debugMessage +=
                        $"[send] {DateTime.Now:HH:mm:ss}：{BitConverter.ToString(hexArray).Replace("-", "")}{Environment.NewLine}";
                }
                catch (Exception e)
                {
                    await PopupService.EnqueueSnackbarAsync(new SnackbarOptions()
                    {
                        Content = e.Message,
                        Type = AlertTypes.Error
                    });
                    return;
                }
            }
            else
            {
                serialPort!.WriteLine(message);
            }

            _debugMessage += $"[info] {DateTime.Now:HH:mm:ss}：{message.TrimEnd('\0')}{Environment.NewLine}";
        }
    }

    private void GoSetting()
    {
        NavigationManager.NavigateTo("/setting");
    }

    public async ValueTask DisposeAsync()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
        disposable = true;
        await Task.CompletedTask;
    }
}