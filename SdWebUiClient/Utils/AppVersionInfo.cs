using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Prism.Mvvm;

namespace SdWebUiClient.Utils;

public class AppVersionInfo : BindableBase
{
    private string title;
    private string version = string.Empty;

    public AppVersionInfo()
    {
        var projectName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
        ProjectName = Regex.Replace(projectName, "([a-z])([A-Z])", "$1 $2");
        Title = ProjectName;

        SetVersion();
        AddDebugMark();
    }

    public string Title
    {
        get => string.IsNullOrWhiteSpace(Version)
            ? title
            : title + "   version : " + Version;
        private set => SetProperty(ref title, value);
    }

    public int MajorVersion { get; init; }

    public int MinorVersion { get; init; }

    public int PatchVersion { get; init; }

    // ReSharper disable once CommentTypo

    /// <summary>
    ///     最終アップデートの日付を `YYYYmmdd` のフォーマットで入力します。
    /// </summary>
    public string Updated { get; init; } = "";

    public string SuffixId { get; init; } = "a";

    private string ProjectName { get; }

    private string Version
    {
        get => version;
        set => SetProperty(ref version, value);
    }

    private void UpdateTitle()
    {
        Version = $"{MajorVersion}.{MinorVersion}.{PatchVersion} ({Updated}{SuffixId})";
    }

    [Conditional("RELEASE")]
    private void SetVersion()
    {
        UpdateTitle();
    }

    [Conditional("DEBUG")]
    private void AddDebugMark()
    {
        Title += " (Debug)";
    }
}