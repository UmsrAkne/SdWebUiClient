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
            var tempFilePath = Path.GetTempFileName();

            // 内容を書き込む
            File.WriteAllText(tempFilePath, content, Encoding.UTF8);
            Console.WriteLine($"一時ファイル作成: {tempFilePath}");

            // FileSystemWatcher の設定
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(tempFilePath)!,
                Filter = Path.GetFileName(tempFilePath),
                NotifyFilter = NotifyFilters.LastWrite,
            };

            watcher.Changed += (sender, e) =>
            {
                Console.WriteLine($"ファイルが変更されました: {e.FullPath}");

                try
                {
                    var updatedContent = File.ReadAllText(tempFilePath, Encoding.UTF8);
                    Console.WriteLine($"新しい内容:\n{updatedContent}");
                }
                catch (IOException)
                {
                    Console.WriteLine("読み込みに失敗しました。別プロセスがファイルをロックしている可能性があります。");
                }
            };

            watcher.EnableRaisingEvents = true;

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Vim\vim91\gvim.exe",
                Arguments = $"\"{tempFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                Process.Start(psi);
                Console.WriteLine("gvim を起動しました。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gvim の起動に失敗しました: {ex.Message}");
            }
        }
    }
}