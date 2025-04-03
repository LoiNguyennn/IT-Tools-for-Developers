using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysis
{
    public class VowelCounter : ITool
    {
        public string Name => "Vowel Counter";
        public string Description => "Counts vowels in the input text";
        public string Category => "Text Analysis Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            int count = input.Count(c => "aeiouAEIOU".Contains(c));
            return count.ToString();
        }
    }
}
