using ITTools.Core.Models;
using System.Web;

namespace UrlEncoder
{
    public class UrlEncoderTool : ITool
    {
        public string Name => "URL Encoder";
        public string Description => "Encodes a URL string";
        public string Category => "Utility Tools";
        public string Execute(string input)
        {
            return HttpUtility.UrlEncode(input);
        }
    }
}
