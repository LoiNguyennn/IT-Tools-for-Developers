using ITTools.Core.Models;
using System.Text;

namespace ImageVideoTools
{
    public class PlaceholderTextGenerator : ITool
    {
        public string Name => "Placeholder Text Generator";
        public string Description => "Generates placeholder text like Lorem Ipsum";
        public string Category => "Image/Video Tools";

        private static readonly string[] Words = ["lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
            "adipiscing", "elit", "sed", "do", "eiusmod", "tempor"];

        private static readonly Random Random = new Random();

        public string Execute(string wordCount)
        {
            if (!int.TryParse(wordCount, out var count) || count < 1)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                sb.Append(Words[Random.Next(Words.Length)]);
                if (i < count - 1)
                    sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}

