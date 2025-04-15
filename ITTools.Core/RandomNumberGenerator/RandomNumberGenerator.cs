using ITTools.Core.Models;

namespace Utility
{
    public class RandomNumberGenerator : ITool
    {
        private readonly Random _random = new Random();

        public string Name => "Random Number Generator";
        public string Description => "Generates a random number within a specified range (input as 'min,max')";
        public string Category => "Utility Tools";
        public string Execute(string input)
        {
            try
            {
                var parts = input.Split(',');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int min) || !int.TryParse(parts[1], out int max))
                {
                    return "Invalid input: use 'min,max' format (e.g., '1,10')";
                }
                return _random.Next(min, max + 1).ToString(); // +1 because Next is exclusive of max
            }
            catch
            {
                return "Error generating random number";
            }
        }
    }
}
