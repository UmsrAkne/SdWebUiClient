using System.Text.Json.Serialization;

namespace SdWebUiClient.Models
{
    public class GenerationState
    {
        [JsonPropertyName("skipped")]
        public bool Skipped { get; set; }

        [JsonPropertyName("interrupted")]
        public bool Interrupted { get; set; }

        [JsonPropertyName("stopping_generation")]
        public bool StoppingGeneration { get; set; }

        [JsonPropertyName("job")]
        public string Job { get; set; }

        [JsonPropertyName("job_count")]
        public int JobCount { get; set; }

        [JsonPropertyName("job_timestamp")]
        public string JobTimestamp { get; set; }

        [JsonPropertyName("job_no")]
        public int JobNo { get; set; }

        [JsonPropertyName("sampling_step")]
        public int SamplingStep { get; set; }

        [JsonPropertyName("sampling_steps")]
        public int SamplingSteps { get; set; }
    }
}