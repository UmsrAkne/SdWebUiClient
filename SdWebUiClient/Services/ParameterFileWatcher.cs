using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SdWebUiClient.Models;
using SdWebUiClient.Utils;
using YamlDotNet.Core;

namespace SdWebUiClient.Services
{
    public class ParameterFileWatcher
    {
        public event EventHandler ParameterFileChanged;

        private readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300);
        private DateTime lastReadTime = DateTime.MinValue;

        public void MonitorTempFile(ImageGenerationParameters imageGenerationParameters)
        {
            // 一時ファイルの作成
            var fileInfo = new FileInfo("generationParams.yaml");

            // 内容を書き込む
            File.WriteAllText(fileInfo.FullName, YamlHelper.ConvertToYaml(imageGenerationParameters), Encoding.UTF8);
            Console.WriteLine($"create temp file. {fileInfo.FullName}");

            // FileSystemWatcher の設定
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fileInfo.FullName)!,
                Filter = Path.GetFileName(fileInfo.FullName),
                NotifyFilter = NotifyFilters.LastWrite,
            };

            watcher.Changed += (sender, e) =>
            {
                var now = DateTime.Now;

                // 最終更新から debounceTime 経過していないならスキップ
                if (now - lastReadTime < debounceTime)
                {
                    return;
                }

                lastReadTime = now;

                Console.WriteLine($"changed file: {e.FullPath}");
                ParameterFileChanged?.Invoke(sender, e);

                try
                {
                    var updatedContent = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
                    Console.WriteLine($"new content:\n{updatedContent}");

                    var newParameters = YamlHelper.LoadFromYaml(fileInfo.FullName);
                    imageGenerationParameters.Prompt = newParameters.Prompt;
                    imageGenerationParameters.NegativePrompt = newParameters.NegativePrompt;
                }
                catch (IOException)
                {
                    Console.WriteLine("Read failed. Another process may have locked the file.");
                }
                catch (SemanticErrorException se)
                {
                    Console.WriteLine("Read failed. Invalid yaml format.");
                    Console.WriteLine(se.Message);
                }
            };

            watcher.EnableRaisingEvents = true;

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Vim\vim91\gvim.exe",
                Arguments = $"\"{fileInfo.FullName}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                Process.Start(psi);
                Console.WriteLine("start gvim");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to start gvim: {ex.Message}");
            }
        }
    }
}