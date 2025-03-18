using ITTools.Core.Models;

namespace TextTrimmer
{
    public class TextTrimmerTool : ITool
    {
        public string Name => "Text Trimmer";
        public string Description => "Removes leading and trailing whitespace from text";
        public string Category => "Utility Tools";
        public string Execute(string input)
        {
            return input.Trim();
        }
    }
}
