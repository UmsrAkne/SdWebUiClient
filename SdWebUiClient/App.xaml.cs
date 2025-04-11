using System.Windows;
using Prism.Ioc;
using SdWebUiClient.ViewModels;
using SdWebUiClient.Views;

namespace SdWebUiClient;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private MainWindowViewModel vm;

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (MainWindow != null)
        {
            // OnAppExit の時点で MainWindow を参照できなかったので、あらかじめビューモデルのインスタンスを取得しておく。
            vm = MainWindow.DataContext as MainWindowViewModel;
        }

        Exit += OnAppExit;
    }

    private void OnAppExit(object sender, ExitEventArgs e)
    {
        vm.ParameterFileWatcher.ExitEditor();
    }
}