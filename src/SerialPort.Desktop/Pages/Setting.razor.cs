using System.Text.Json;
using BlazorComponent;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using SerialPort.Desktop.Modules;

namespace SerialPort.Desktop.Pages;

public partial class Setting
{
    [CascadingParameter(Name = nameof(SettingOptions))]
    public SettingOptions _settingOptions { get; set; }

    private List<DebugFormLocationType> DebugFormLocations = new()
    {
        DebugFormLocationType.left,
        DebugFormLocationType.right
    };

    private void GoHome()
    {
        NavigationManager.NavigateTo("/");
    }

    private async Task OnSave()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "setting.json");
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(_settingOptions));
            await _PopupService.EnqueueSnackbarAsync("保存成功", AlertTypes.Success);
        }
        catch (Exception e)
        {
            await _PopupService.EnqueueSnackbarAsync("保存失败：" + e.Message, AlertTypes.Error);
        }
    }
}