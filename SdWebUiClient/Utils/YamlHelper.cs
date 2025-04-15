using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, ConvertToYaml(parameters));
        }

        public static string ConvertToYaml(ImageGenerationParameters parameters)
        {
            var prompt = parameters.Prompt;
            prompt = FormatInput(prompt);
            prompt = FormatBlock(prompt);

            var negativePrompt = parameters.NegativePrompt;
            negativePrompt = FormatInput(negativePrompt);
            negativePrompt = FormatBlock(negativePrompt);

            parameters.Prompt = string.Empty;
            parameters.NegativePrompt = string.Empty;
            var yaml = Serializer.Serialize(parameters);

            yaml = Regex.Replace(yaml, "^prompt: ''", $"prompt: |{Environment.NewLine}{prompt}");
            yaml = Regex.Replace(yaml, "negativePrompt: ''", $"negativePrompt: |{Environment.NewLine}{negativePrompt}");

            return yaml;
        }

        /// <summary>
        /// 指定した yaml ファイルを読み込んで、テキストをフォーマット。ImageGenerationParameters に変換します。
        /// </summary>
        /// <param name="filePath">yaml ファイルのパス</param>
        /// <returns>yaml ファイルから変換されたオブジェクト。</returns>
        public static ImageGenerationParameters LoadFromYaml(string filePath)
        {
            var yaml = File.ReadAllText(filePath);
            return ConvertFromYaml(yaml);
        }

        /// <summary>
        /// 指定した yaml のテキストを ImageGenerationParameters に変換します。
        /// </summary>
        /// <param name="yamlText">yaml のフォーマットに沿ったテキスト</param>
        /// <returns>yaml テキストから変換されたオブジェクト。</returns>
        public static ImageGenerationParameters ConvertFromYaml(string yamlText)
        {
            var igp = Deserializer.Deserialize<ImageGenerationParameters>(yamlText);
            igp.Prompt = FormatInput(igp.Prompt);
            igp.NegativePrompt = FormatInput(igp.NegativePrompt);
            return igp;
        }

        public static string ConvertCustomWeightSyntax(string prompt)
        {
            var r = string.Empty;
            var sr = new StringReader(prompt);
            while (sr.ReadLine() is { } line)
            {
                r += Result(line).Trim() + Environment.NewLine;
            }

            r = FormatInput(r);

            return r;

            string Result(string input)
            {
                var prompts = input.Split(",");
                var resultList = new List<string>();
                foreach (var p in prompts)
                {
                    var pt = p.Trim();

                    // 元からの括弧付きはスキップ
                    if ((pt.StartsWith("(") && pt.EndsWith(")")) || (pt.StartsWith("<") && pt.EndsWith(">")))
                    {
                        resultList.Add(p);
                        continue;
                    }

                    var m = Regex.Match(pt, @"(.+:)(\d+)");
                    if (!m.Success)
                    {
                        resultList.Add(p);
                        continue;
                    }

                    if (!int.TryParse(m.Groups[2].Value, out var weight))
                    {
                        resultList.Add(p);
                        continue;
                    }

                    if (weight >= 5)
                    {
                        var weightString = (weight / 100.0).ToString("F2");
                        weightString = Regex.Replace(weightString, "0+$", "0");
                        resultList.Add($"({m.Groups[1]}{weightString})");
                    }
                }

                return string.Join(", ", resultList);
            }
        }

        private static string FormatBlock(string block)
        {
            var r = new StringReader(block);
            block = string.Empty;
            while (r.ReadLine() is { } line)
            {
                block += "  " + line + Environment.NewLine;
            }

            return block.TrimEnd();
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