using ITTools.Core.Models;
using System.Text;

namespace EncodingTools
{
    public class Base64Encoder : ITool
    {
        public string Name => "Base64 Encoder";
        public string Description => "Encodes text to Base64 format";
        public string Category => "Encoding Tools";
        public string Execute(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
    }   
}