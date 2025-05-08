using ITTools.Core.Models;
using Figgle;

namespace ImageVideoTools
{
    public class ASCIIArtGenerator : ITool
    {
        public string Name => "ASCII Art Generator";
        public string Description => "Creates simple ASCII art from text input";
        public string Category => "Image/Video Tools";

        public string Execute(string input)
        {
            return FiggleFonts.Standard.Render(input);
        }
    }
}