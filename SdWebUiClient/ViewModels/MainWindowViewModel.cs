using System;
using System.Diagnostics;
using System.IO;
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
    private readonly GenRequestDispatcher genRequestDispatcher = new ();
    private ProgressResponse currentProgressResponse;

    public MainWindowViewModel()
    {
        SetDummies();
        LoadDefault();

        ParameterFileWatcher.ParameterFileChanged += (_, _) =>
        {
            RequestGenImageAsyncCommand.Execute();
        };

        dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        dispatcherTimer.Tick += (_, _) =>
        {
            GetProgressCommand.Execute(null);
        };

        dispatcherTimer.Start();
    }

    public AppVersionInfo AppVersionInfo { get; set; } = new ();

    public ImageGenerationParameters ImageGenerationParameters { get; set; } = new ();

    public ParameterFileWatcher ParameterFileWatcher { get; set; } = new ();

    public ProgressResponse CurrentProgressResponse
    {
        get => currentProgressResponse;
        set => SetProperty(ref currentProgressResponse, value);
    }

    public DelegateCommand OpenEditorCommand => new DelegateCommand(() =>
    {
        ParameterFileWatcher.MonitorTempFile(ImageGenerationParameters);
    });

    public DelegateCommand RequestGenImageAsyncCommand => new DelegateCommand(() =>
    {
        if (ImageGenerationParameters.HasInvalidValues())
        {
            Console.WriteLine("ImageGenerationParameters contains invalid value.");
            return;
        }

        genRequestDispatcher.EnqueueRequest(ImageGenerationParameters);
    });

    public AsyncDelegateCommand GetProgressCommand => new (async () =>
    {
        CurrentProgressResponse = await genRequestDispatcher.GetProgress();
        Console.WriteLine(CurrentProgressResponse.Progress);
    });

    private void LoadDefault()
    {
        var defaultT2IParameterFile = new FileInfo("yamlFiles/default_t2i_params.yaml");
        if (!defaultT2IParameterFile.Exists)
        {
            YamlHelper.SaveToYaml(new ImageGenerationParameters(), defaultT2IParameterFile.FullName);
        }

        ImageGenerationParameters = YamlHelper.LoadFromYaml(defaultT2IParameterFile.FullName);
    }

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        ImageGenerationParameters.Prompt = "generation Parameters, test, test1, test2";
        ImageGenerationParameters.NegativePrompt = "negative Prompts, generation Parameters, test, test1, test2";
    }
}