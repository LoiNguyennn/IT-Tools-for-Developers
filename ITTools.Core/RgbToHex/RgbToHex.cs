using ITTools.Core.Models;
using System.Text.RegularExpressions;

namespace ColorTools
{
    public class RgbToHex : ITool
    {
        public string Name => "RGB to Hex Converter";

        public string Description =>
            "Converts an RGB color value (e.g., 'rgb(255, 100, 50)' or '255, 100, 50') into a hexadecimal color code.";

        public string Category => "Color Tools";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "No input provided.";

            // Extract numbers from formats like "rgb(255, 100, 50)" or "255,100,50"
            var match = Regex.Match(input, @"(\d{1,3})\D+(\d{1,3})\D+(\d{1,3})");

            if (!match.Success || match.Groups.Count < 4)
                return "Invalid RGB format. Please use 'rgb(255, 0, 0)' or '255, 0, 0'.";

            if (!int.TryParse(match.Groups[1].Value, out int r) ||
                !int.TryParse(match.Groups[2].Value, out int g) ||
                !int.TryParse(match.Groups[3].Value, out int b))
                return "Invalid RGB values.";

            if (!IsInRange(r) || !IsInRange(g) || !IsInRange(b))
                return "RGB values must be between 0 and 255.";

            string hex = $"#{r:X2}{g:X2}{b:X2}";
            return hex;
        }

        private bool IsInRange(int value) => value >= 0 && value <= 255;
    }
}