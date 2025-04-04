using Prism.Mvvm;
using SdWebUiClient.Utils;

namespace SdWebUiClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public AppVersionInfo AppVersionInfo { get; set; } = new ();
}