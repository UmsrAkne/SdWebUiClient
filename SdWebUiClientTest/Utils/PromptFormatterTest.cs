using SdWebUiClient.Utils;

namespace SdWebUiClientTest.Utils
{
    [TestFixture]
    public class PromptFormatterTest
    {
        private static IEnumerable<TestCaseData> PromptFormatTestCases()
        {
            yield return new TestCaseData(
                "test, test:0.10",
                "test, test:0.1"
            ).SetName("通常入力");

            yield return new TestCaseData(
                "test, test:0.10, test:0.200",
                "test, test:0.1, test:0.2"
            ).SetName("通常入力、複数箇所置き換え");

            yield return new TestCaseData(
                "test, test:0.10,\r\n test:0.200",
                "test, test:0.1,\r\n test:0.2"
            ).SetName("通常入力、複数箇所置き換え、改行あり");

            yield return new TestCaseData(
                "test, test:0.1",
                "test, test:0.1"
            ).SetName("置き換え対象なし");

            yield return new TestCaseData(string.Empty, string.Empty
            ).SetName("empty string.");

            yield return new TestCaseData(null, null
            ).SetName("null.");
        }

        [TestCaseSource(nameof(PromptFormatTestCases))]
        public void FinalizeFormat(string input, string expect)
        {
            var actual = PromptFormatter.FinalizeFormat(input);
            Assert.That(actual, Is.EqualTo(expect));
        }
    }
}