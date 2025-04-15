using ITTools.Core.Models;

namespace TokenGenerators
{
    public class RandomTokenGenerator : ITool
    {
        public string Name => "Random Token Generator";
        public string Description => "Generates a random alphanumeric token";
        public string Category => "Token Generation Tools";

        public string Execute(string input)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const int length = 16; // Default token length
            char[] token = new char[length];
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                token[i] = chars[random.Next(chars.Length)];
            }

            return new string(token);
        }
    }
}