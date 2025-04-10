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
    }
}