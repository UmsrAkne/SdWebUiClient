using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SdWebUiClient.Models;

namespace SdWebUiClient.Services
{
    public static class GenRequestDispatcher
    {
        public static async Task RequestT2I(ImageGenerationParameters parameters)
        {
            const string url = "http://127.0.0.1:7860/sdapi/v1/txt2img";

            // リクエストボディの定義
            var payload = new
            {
                prompt = parameters.Prompt,
                steps = parameters.Steps,
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

        public static async Task<ProgressResponse> GetProgress()
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

        private static DirectoryInfo CreateDirectory()
        {
            var outputDirectory = new DirectoryInfo($"output\\{DateTime.Today:yyyyMMdd}");
            outputDirectory.Create();
            return outputDirectory;
        }
    }
}