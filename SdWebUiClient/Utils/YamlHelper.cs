using System;
using System.IO;
using System.Linq;
using SdWebUiClient.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SdWebUiClient.Utils
{
    public static class YamlHelper
    {
        // ReSharper disable once ArrangeModifiersOrder
        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // ReSharper disable once ArrangeModifiersOrder
        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public static void SaveToYaml(ImageGenerationParameters parameters, string filePath)
        {
            var yaml = Serializer.Serialize(parameters);
            File.WriteAllText(filePath, yaml);
        }

        public static ImageGenerationParameters LoadFromYaml(string filePath)
        {
            var yaml = File.ReadAllText(filePath);
            return ConvertFromYaml(yaml);
        }

        public static ImageGenerationParameters ConvertFromYaml(string yamlText)
        {
            var igp = Deserializer.Deserialize<ImageGenerationParameters>(yamlText);
            igp.Prompt = FormatInput(igp.Prompt);
            igp.NegativePrompt = FormatInput(igp.NegativePrompt);
            return igp;
        }

        private static string FormatInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // 1. 各行に対してタブを削除
            var lines = input
                .Replace("\r\n", "\n") // 統一
                .Split('\n')
                .Select(line => line.Replace("\t", string.Empty).TrimEnd())
                .ToList();

            // 2. 先頭・末尾の空白／空行を削除
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
            {
                lines.RemoveAt(0);
            }

            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            // 3. 行ごとに末尾にカンマを追加（空行は除く）
            for (var i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                // 各行を "a, b, c" の形式に整える
                var parts = lines[i]
                    .Split(',')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                lines[i] = string.Join(", ", parts) + ",";
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}