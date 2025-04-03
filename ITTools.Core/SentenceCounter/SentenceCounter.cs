using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysis
{
    public class SentenceCounter : ITool
    {
        public string Name => "Sentence Counter";
        public string Description => "Counts sentences in the input";
        public string Category => "Text Analysis Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            // Split by sentence-ending punctuation and remove empty entries
            var sentences = input.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            return sentences.Length.ToString();
        }
    }
}
