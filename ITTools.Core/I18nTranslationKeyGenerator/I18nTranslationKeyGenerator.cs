using ITTools.Core.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StringUtilities
{
    public class I18NTranslationKeyGenerator : ITool
    {
        public string Name => "I18N Translation Key Generator";

        public string Description => "Converts a UI string into a translation key, e.g., 'Create Account' → 'auth.create_account'.";

        public string Category => "String Utilities";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Input is empty.";

            // Remove leading/trailing spaces and lowercase
            var normalized = input.Trim().ToLowerInvariant();

            // Replace non-word characters and spaces with underscore
            var key = Regex.Replace(normalized, @"[^\w]+", "_");

            // Trim underscores
            key = key.Trim('_');

            // Optionally, add a default namespace prefix
            return $"general.{key}";
        }
    }
}