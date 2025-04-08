using System.IO;
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
            return Deserializer.Deserialize<ImageGenerationParameters>(yaml);
        }
    }
}