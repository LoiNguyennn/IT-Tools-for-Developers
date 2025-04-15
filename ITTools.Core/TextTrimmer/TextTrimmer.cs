using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class TextTrimmer : ITool
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
