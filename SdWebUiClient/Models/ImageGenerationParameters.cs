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
        private int steps = 12;

        public string Prompt { get => prompt; set => SetProperty(ref prompt, value); }

        public string NegativePrompt { get => negativePrompt; set => SetProperty(ref negativePrompt, value); }

        public int Width { get => width; set => SetProperty(ref width, value); }

        public int Height { get => height; set => SetProperty(ref height, value); }

        public int BatchSize { get => batchSize; set => SetProperty(ref batchSize, value); }

        public int BatchCount { get => batchCount; set => SetProperty(ref batchCount, value); }

        public int Steps { get => steps; set => SetProperty(ref steps, value); }

        public int Seed { get => seed; set => SetProperty(ref seed, value); }

        public bool HasInvalidValues()
        {
            if (Width < 64 || Width > 2560 || Width % 16 != 0)
            {
                return true;
            }

            if (Height < 64 || Height > 2560 || Height % 16 != 0)
            {
                return true;
            }

            // 総面積でこれを上回る画像は生成しない。
            if (Width * Height > 2560 * 1680)
            {
                return true;
            }

            // バッチサイズは一般に小さな数。
            if (BatchSize is < 1 or > 8)
            {
                return true;
            }

            // バッチ回数もそれほど多くしない。
            if (BatchCount is < 1 or > 8)
            {
                return true;
            }

            if (Steps is < 5 or > 30)
            {
                return true;
            }

            // シードは -1（ランダム） か 0以上の値を許可
            if (Seed < -1)
            {
                return true;
            }

            // すべて妥当
            return false;
        }
    }
}