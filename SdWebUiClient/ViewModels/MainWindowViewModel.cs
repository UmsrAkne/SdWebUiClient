using System.Diagnostics;
using Prism.Mvvm;
using SdWebUiClient.Models;
using SdWebUiClient.Utils;

namespace SdWebUiClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public MainWindowViewModel()
    {
        SetDummies();

        ParameterFileWatcher.ParameterFileChanged += (sender, args) =>
        {
            _ = GenRequestDispatcher.RequestT2I(ImageGenerationParameters).ContinueWith(
                t =>
                {
                    if (t.Exception != null)
                    {
                        Console.WriteLine("Image Generation Failed.");
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        };
    }

    public AppVersionInfo AppVersionInfo { get; set; } = new ();

    public ImageGenerationParameters ImageGenerationParameters { get; set; } = new ();

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        ImageGenerationParameters.Prompt = "generation Parameters, test, test1, test2";
        ImageGenerationParameters.NegativePrompt = "negative Prompts, generation Parameters, test, test1, test2";
    }
}