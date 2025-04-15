using ITTools.Core.Models;

namespace StringProcessing
{
    public class ReverseString : ITool
    {
        public string Name => "Reverse String";
        public string Description => "Reverses a given string.";

        public string Category => "String Tools";

        public string Execute(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
