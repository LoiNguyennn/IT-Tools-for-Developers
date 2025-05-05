using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public class UrlEncoder : ITool
    {
        public string Name => "Url Encoder";
        public string Description => "Encodes a URL string";
        public string Category => "Utility Tools";
        public string Execute(string input)
        {
            return HttpUtility.UrlEncode(input);
        }
    }
}
