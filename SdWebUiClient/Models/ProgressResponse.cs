using System.Text.Json.Serialization;

namespace SdWebUiClient.Models
{
    public class ProgressResponse
    {
        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        [JsonPropertyName("eta_relative")]
        public double EtaRelative { get; set; }

        [JsonPropertyName("state")]
        public GenerationState State { get; set; }

        [JsonPropertyName("current_image")]
        public string? CurrentImage { get; set; }

        [JsonPropertyName("textinfo")]
        public string? TextInfo { get; set; }

        public string StatusText
        {
            get
            {
                if (Progress == 0)
                {
                    return string.Empty;
                }

                return $"Progress : {Progress * 100:F0}%";
            }
        }

        /// <summary>
        /// このレスポンスの中に進行状況の情報が含まれているかどうかを取得します。
        /// </summary>
        public bool IsEmpty => StatusText == string.Empty;
    }
}