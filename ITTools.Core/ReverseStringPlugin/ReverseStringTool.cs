using ITTools.Core.Models;
using System;

namespace ReverseStringPlugin
{
    public class ReverseStringTool : ITool
    {
        public string Name => "Reverse String";
        public string Description => "Reverses a given string.";

        public string Execute(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}