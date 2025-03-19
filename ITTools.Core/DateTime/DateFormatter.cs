using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimePlugins
{
    public class DateFormatter : ITool
    {
        public string Name => "Date Formatter";
        public string Description => "Formats the current date to YYYY-MM-DD";
        public string Category => "Date/Time Tools";
        public string Execute(string input)
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
