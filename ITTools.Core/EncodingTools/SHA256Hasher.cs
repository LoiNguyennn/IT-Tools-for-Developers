using ITTools.Core.Models;
using System.Text;
using System.Security.Cryptography;

namespace EncodingTools
{
    public class Sha256Hasher : ITool
    {
        public string Name => "SHA256 Hasher";
        public string Description => "Generates a SHA256 hash of the input";
        public string Category => "Encoding Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = SHA256.HashData(inputBytes); // Static method for SHA-256

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // Convert each byte to 2-char hex
            }

            return sb.ToString();
        }
    }
}