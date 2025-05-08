using ITTools.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace EncodingTools
{
    public class MD5Hasher : ITool
    {
        public string Name => "MD5 Hasher";
        public string Description => "Generates an MD5 hash of the input";
        public string Category => "Encoding Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes); // Call static method HashData instead of ComputeHash

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}