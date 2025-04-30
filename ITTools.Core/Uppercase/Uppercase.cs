using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringProcessing
{
    public class Uppercase : ITool
    {
        public string Name => "Uppercase";
        public string Description => "Converts a given string to uppercase.";

        public string Category => "String Tools";
        public string Execute(string input)
        {
            return input.ToUpper();
        }
    }
}
