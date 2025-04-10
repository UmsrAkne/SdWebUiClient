using System;
using System.Diagnostics;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using SdWebUiClient.Commands;
using SdWebUiClient.Models;
using SdWebUiClient.Services;
using SdWebUiClient.Utils;

namespace SdWebUiClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly DispatcherTimer dispatcherTimer = new ();

    public MainWindowViewModel()
    {
        SetDummies();

        ParameterFileWatcher.ParameterFileChanged += (_, _) =>
        {
            RequestGenImageAsyncCommand.Execute(null);
        };

        dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
        dispatcherTimer.Tick += (_, _) =>
        {
            GetProgressCommand.Execute(null);
        };

        dispatcherTimer.Start();
    }

    public AppVersionInfo AppVersionInfo { get; set; } = new ();

    public ImageGenerationParameters ImageGenerationParameters { get; set; } = new ();

    public ParameterFileWatcher ParameterFileWatcher { get; set; } = new ();

    public DelegateCommand OpenEditorCommand => new DelegateCommand(() =>
    {
        ParameterFileWatcher.MonitorTempFile(ImageGenerationParameters);
    });

    public AsyncDelegateCommand RequestGenImageAsyncCommand => new AsyncDelegateCommand(async () =>
    {
        await GenRequestDispatcher.RequestT2I(ImageGenerationParameters);
    });

    public AsyncDelegateCommand GetProgressCommand => new (async () =>
    {
        var progress = await GenRequestDispatcher.GetProgress();
        Console.WriteLine(progress.Progress);
    });

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        ImageGenerationParameters.Prompt = "generation Parameters, test, test1, test2";
        ImageGenerationParameters.NegativePrompt = "negative Prompts, generation Parameters, test, test1, test2";
    }
}