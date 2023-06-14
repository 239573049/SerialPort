namespace SerialPort.Desktop.Modules;

public class CheckBitDto
{
    public CheckBitType Value { get; set; }

    public CheckBitDto(CheckBitType value)
    {
        Value = value;
    }
}

public enum CheckBitType
{
    None,

    Odd,

    Even,
}