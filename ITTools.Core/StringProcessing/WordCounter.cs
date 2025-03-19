using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringProcessing
{
    public class WordCounter : ITool
    {
        public string Name => "Word Counter";
        public string Description => "Counts the number of words in the input";
        public string Category => "String Processing Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            // Split by whitespace (space, tab, newline) and filter out empty entries
            var words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length.ToString();
        }
    }
}
