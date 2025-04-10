using SdWebUiClient.Models;
using SdWebUiClient.Utils;

namespace SdWebUiClientTest.Utils
{
    public class YamlHelperTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void ConvertFromYaml()
        {
            const string yaml = @"
                prompt: |
                    text1,  text2,
                    text3,
                negativePrompt: |
                    negativeText1,

                    negativeText2,  param text,
                seed: 100
            ";

            var obj = YamlHelper.ConvertFromYaml(yaml);

            Assert.Multiple(() =>
            {
                Assert.That(obj.Prompt, Is.EqualTo("text1, text2,\r\ntext3,"));
                Assert.That(obj.NegativePrompt, Is.EqualTo("negativeText1,\r\n\r\nnegativeText2, param text,"));
            });
        }

        [Test]
        public void ConvertToYaml()
        {
            var igp = new ImageGenerationParameters()
            {
                Prompt = "text1,  text2,\r\ntext3,",
                NegativePrompt = "negativeText1,\r\n\r\nnegativeText2,\r\nparam text,",
            };

            var actual = YamlHelper.ConvertToYaml(igp);
            System.Diagnostics.Debug.WriteLine($"{actual}(YamlHelperTest : 53)");

            const string expect =
                    "prompt: |\r\n"
                    + "  text1, text2,\r\n"
                    + "  text3,\r\n"
                    + "negativePrompt: |\r\n"
                    + "  negativeText1,\r\n"
                    + "  \r\n"
                    + "  negativeText2,\r\n"
                    + "  param text,\r\n"
                    + "width: 512\r\n"
                    + "height: 512\r\n"
                    + "batchSize: 1\r\n"
                    + "batchCount: 1\r\n"
                    + "steps: 12\r\n"
                    + "seed: -1\r\n"
                ;

            Assert.That(actual, Is.EqualTo(expect));
        }

        [TestCase("a:90", "(a:0.90),")]
        [TestCase("a, b:90", "a, (b:0.90),")]
        [TestCase("a:105", "(a:1.05),")]
        [TestCase("a:90, b:80", "(a:0.90), (b:0.80),")]
        [TestCase("a:90,\r\n b:80\r\n", "(a:0.90),\r\n(b:0.80),")]
        [TestCase("(a:1.0)", "(a:1.0),")]
        [TestCase("<a:1.0>", "<a:1.0>,")]
        public void ConvertCustomWeightSyntax(string input, string expected)
        {
            var result = YamlHelper.ConvertCustomWeightSyntax(input);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}