using ITTools.Core.Models;
using System;
using System.Text.RegularExpressions;

namespace ConversionTools
{
    public class HexToDecimal : ITool
    {
        public string Name => "Hex To Decimal";
        public string Description => "Converts hexadecimal string to decimal";
        public string Category => "Conversion Tools";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Input is empty.";

            input = input.Trim();

            if (!Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z"))
                return $"Invalid hexadecimal input: '{input}'";

            try
            {
                return Convert.ToInt32(input, 16).ToString();
            }
            catch (Exception ex)
            {
                return $"Conversion failed: {ex.Message}";
            }
        }
    }
}
