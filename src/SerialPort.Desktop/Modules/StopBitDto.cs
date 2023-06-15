namespace SerialPort.Desktop.Modules;

public class StopBitDto
{
    public System.IO.Ports.StopBits Value { get; set; }

    public StopBitDto(System.IO.Ports.StopBits value)
    {
        Value = value;
    }
}