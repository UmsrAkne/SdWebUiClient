using Prism.Mvvm;

namespace SdWebUiClient.Models
{
    public class ImageGenerationParameters : BindableBase
    {
        private string prompt = string.Empty;
        private string negativePrompt = string.Empty;
        private int width = 512;
        private int height = 512;
        private int batchSize = 1;
        private int batchCount = 1;
        private int seed = -1;

        public string Prompt { get => prompt; set => SetProperty(ref prompt, value); }

        public string NegativePrompt { get => negativePrompt; set => SetProperty(ref negativePrompt, value); }

        public int Width { get => width; set => SetProperty(ref width, value); }

        public int Height { get => height; set => SetProperty(ref height, value); }

        public int BatchSize { get => batchSize; set => SetProperty(ref batchSize, value); }

        public int BatchCount { get => batchCount; set => SetProperty(ref batchCount, value); }

        public int Seed { get => seed; set => SetProperty(ref seed, value); }
    }
}