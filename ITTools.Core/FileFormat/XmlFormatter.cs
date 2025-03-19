using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileFormat
{
    public class XmlFormatter : ITool
    {
        public string Name => "XML Formatter";
        public string Description => "Formats an XML string with proper structure";
        public string Category => "File Format Tools";
        public string Execute(string input)
        {
            try
            {
                var doc = XDocument.Parse(input);
                return doc.ToString();
            }
            catch
            {
                return "Invalid XML";
            }
        }
    }
}
