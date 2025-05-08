using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace FileFormat
{
    public class JSONFormatter : ITool
    {
        public string Name => "JSON Formatter";
        public string Description => "Formats a JSON string with proper indentation";
        public string Category => "File Format Tools";
        public string Execute(string input)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject(input);
                return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return "Invalid JSON";
            }
        }
    }
}
