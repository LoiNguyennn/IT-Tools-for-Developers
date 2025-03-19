using ITTools.Core.Models;

namespace NumberPlugins
{
    public class PrimeNumberChecker : ITool
    {
        public string Name => "Prime Number Checker";
        public string Description => "Checks if a number is prime";
        public string Category => "Number Tools";
        public string Execute(string input)
        {
            if (!int.TryParse(input, out int num) || num < 2)
            {
                return "Not Prime (invalid input or less than 2)";
            }

            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    return "Not Prime";
                }
            }
            return "Prime";
        }
    }
}
