using ITTools.Core.Models;
using System;

namespace ConversionTools
{
    public class HexToDecimal : ITool
    {
        public string Name => "Hex to Decimal";
        public string Description => "Converts hexadecimal string to decimal";
        public string Category => "Conversion Tools";

        public string Execute(string input)
        {
            return Convert.ToInt32(input, 16).ToString();
        }
    }   
}