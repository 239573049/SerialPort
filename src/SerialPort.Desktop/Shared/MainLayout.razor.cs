namespace SerialPort.Desktop.Shared;

public partial class MainLayout
{
    private SettingOptions _settingOptions = new SettingOptions
    {
        DebugFormLocationType = DebugFormLocationType.left
    };

    protected override async Task OnInitializedAsync()
    {
        
        var path = Path.Combine(AppContext.BaseDirectory, "setting.json");
        if (File.Exists(path))
        {
            try
            {
                var json = await File.ReadAllTextAsync(path);
            
                _settingOptions = JsonSerializer.Deserialize<SettingOptions>(json);
            }
            catch (Exception e)
            {
            }
        }
    }
}