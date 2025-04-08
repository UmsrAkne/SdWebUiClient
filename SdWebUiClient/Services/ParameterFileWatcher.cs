using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SdWebUiClient.Services
{
    public class ParameterFileWatcher
    {
        public void MonitorTempFile(string content)
        {
            // 一時ファイルの作成
            var fileInfo = new FileInfo("generationParams.yaml");

            // 内容を書き込む
            File.WriteAllText(fileInfo.FullName, content, Encoding.UTF8);
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
                Console.WriteLine($"changed file: {e.FullPath}");

                try
                {
                    var updatedContent = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
                    Console.WriteLine($"new content:\n{updatedContent}");
                }
                catch (IOException)
                {
                    Console.WriteLine("Read failed. Another process may have locked the file.");
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