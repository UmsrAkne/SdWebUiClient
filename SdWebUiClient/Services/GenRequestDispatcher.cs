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
            };

            using var httpClient = new HttpClient();

            try
            {
                // POST リクエストを送信
                var response = await httpClient.PostAsJsonAsync(url, payload);
                response.EnsureSuccessStatusCode();

                // レスポンスの JSON をパース
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var base64 = doc.RootElement.GetProperty("images")[0].GetString();

                // base64 → バイナリ変換してファイル保存
                if (base64 != null)
                {
                    var imageBytes = Convert.FromBase64String(base64);
                    await File.WriteAllBytesAsync("output.png", imageBytes);
                }

                Console.WriteLine("Image saved as output.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task GetProgress()
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
                Console.WriteLine("response:\n" + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }
        }
    }
}