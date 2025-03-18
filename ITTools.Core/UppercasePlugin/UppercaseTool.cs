using ITTools.Core.Models;
using System;

namespace UppercasePlugin
{
    public class UppercaseTool : ITool
    {
        public string Name => "Uppercase Converter";
        public string Description => "Converts a given string to uppercase.";

        public string Category => "String Tools";
        public string Execute(string input)
        {
            return input.ToUpper();
        }
    }
}
