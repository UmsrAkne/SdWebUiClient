using System.Text.RegularExpressions;

namespace SdWebUiClient.Utils
{
    public class PromptFormatter
    {
        public static string FinalizeFormat(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return prompt;
            }

            var replaced = Regex.Replace(prompt, @"(\.\d)0+", "$1");
            return replaced;
        }
    }
}