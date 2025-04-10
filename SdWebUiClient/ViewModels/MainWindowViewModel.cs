using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using SdWebUiClient.Commands;
using SdWebUiClient.Models;
using SdWebUiClient.Services;
using SdWebUiClient.Utils;

namespace SdWebUiClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public MainWindowViewModel()
    {
        SetDummies();

        ParameterFileWatcher.ParameterFileChanged += (_, _) =>
        {
            RequestGenImageAsyncCommand.Execute(null);
        };
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
        await GenRequestDispatcher.GetProgress();
    });

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        ImageGenerationParameters.Prompt = "generation Parameters, test, test1, test2";
        ImageGenerationParameters.NegativePrompt = "negative Prompts, generation Parameters, test, test1, test2";
    }
}