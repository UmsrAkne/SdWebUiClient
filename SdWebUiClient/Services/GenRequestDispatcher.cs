using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SdWebUiClient.Events;
using SdWebUiClient.Models;
using SdWebUiClient.Utils;

namespace SdWebUiClient.Services
{
    public class GenRequestDispatcher
    {
        private readonly Queue<ImageGenerationParameters> requestQueue = new();
        private readonly object queueLock = new();
        private bool isProcessing;

        public static event EventHandler RequestCompleted;

        public async Task<ProgressResponse> GetProgress()
        {
            try
            {
                var url = "http://127.0.0.1:7860/sdapi/v1/progress?skip_current_image=false";

                // リクエスト送信
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");

                using var httpClient = new HttpClient();

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
                var result = JsonSerializer.Deserialize<ProgressResponse>(json, options);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: Connection failed to server.");
                return new ProgressResponse()
                {
                   TextInfo = "Generation Failed.",
                };
            }
        }

        public void EnqueueRequest(ImageGenerationParameters parameters)
        {
            lock (queueLock)
            {
                requestQueue.Enqueue(parameters);

                // まだ動いてないなら処理を開始
                if (!isProcessing)
                {
                    _ = ProcessQueueAsync();
                }
            }
        }

        private static DirectoryInfo CreateDirectory()
        {
            var outputDirectory = new DirectoryInfo($"output\\{DateTime.Today:yyyyMMdd}");
            outputDirectory.Create();
            return outputDirectory;
        }

        private async Task RequestT2I(ImageGenerationParameters parameters)
        {
            const string url = "http://127.0.0.1:7860/sdapi/v1/txt2img";

            var p = PromptFormatter.FinalizeFormat(parameters.Prompt);
            var np = PromptFormatter.FinalizeFormat(parameters.NegativePrompt);

            // リクエストボディの定義
            var payload = new
            {
                prompt = p,
                steps = np,
                width = parameters.Width,
                height = parameters.Height,
                batch_size = parameters.BatchSize,
                batchCount = parameters.BatchCount,
            };

            var dir = CreateDirectory();
            using var httpClient = new HttpClient();

            try
            {
                // POST リクエストを送信
                var response = await httpClient.PostAsJsonAsync(url, payload);
                response.EnsureSuccessStatusCode();

                // レスポンスの JSON をパース
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                var count = 1;
                foreach (var jsonElement in doc.RootElement.GetProperty("images").EnumerateArray())
                {
                    var b64 = jsonElement.GetString();
                    if (b64 == null)
                    {
                        continue;
                    }

                    var imageBytes = Convert.FromBase64String(b64);
                    await File.WriteAllBytesAsync($"{dir}\\{DateTime.Now:HHmmss_}{count:00}.png", imageBytes);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ProcessQueueAsync()
        {
            isProcessing = true;

            while (true)
            {
                ImageGenerationParameters parameters;

                lock (queueLock)
                {
                    if (requestQueue.Count == 0)
                    {
                        isProcessing = false;
                        return;
                    }

                    parameters = requestQueue.Dequeue();
                }

                try
                {
                    await RequestT2I(parameters);
                    RequestCompleted?.Invoke(this, new RequestCompletedEventArgs(null));
                }
                catch (Exception ex)
                {
                    RequestCompleted?.Invoke(this, new RequestCompletedEventArgs(ex));
                }
            }
        }
    }
}