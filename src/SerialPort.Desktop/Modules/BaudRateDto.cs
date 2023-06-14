namespace SerialPort.Desktop.Modules;

public class BaudRateDto
{
    public int Value { get; set; }
    
    public BaudRateDto(int value)
    {
        Value = value;
    }
}