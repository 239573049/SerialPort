namespace SerialPort.Desktop.Modules;

public class SerialPortDto
{
    public Guid Id { get; set; }

    /// <summary>
    /// 串口名称
    /// </summary>
    public string Name { get; set; }

    public SerialPortDto(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}