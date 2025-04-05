using Prism.Mvvm;
using SdWebUiClient.Models;
using SdWebUiClient.Utils;

namespace SdWebUiClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public AppVersionInfo AppVersionInfo { get; set; } = new ();

    public ImageGenerationParameters ImageGenerationParameters { get; set; } = new ();
}