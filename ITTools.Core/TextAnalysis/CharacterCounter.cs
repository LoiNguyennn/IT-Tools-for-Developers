using ITTools.Core.Models;

namespace TextAnalysis
{
    public class CharacterCounter : ITool
    {
        public string Name => "Character Counter";
        public string Description => "Counts characters in the input";
        public string Category => "Text Analysis Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            return input.Length.ToString();
        }
    }
}
