using ITTools.Core.Models;
using System.Text.RegularExpressions;

namespace ITTools.Core.Tools
{
    public class HexToRgbConverter : ITool
    {
        public string Name => "Hex to RGB Converter";

        public string Description =>
            "Converts a hexadecimal color code (e.g., '#FF5733' or 'FF5733') to an RGB value.";

        public string Category => "Color Tools";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "No input provided.";

            string hex = input.Trim().TrimStart('#');

            // Valid lengths are 6 (RRGGBB) or 3 (RGB shorthand)
            if (hex.Length == 3)
            {
                // Expand shorthand: f60 → ff6600
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            }

            if (!Regex.IsMatch(hex, @"^[0-9a-fA-F]{6}$"))
                return "Invalid hex format. Please enter a 3- or 6-digit hex code (with or without #).";

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return $"RGB({r}, {g}, {b})";
        }
    }
}