using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberPlugins
{
    public class FactorialCalculator : ITool
    {
        public string Name => "Factorial Calculator";
        public string Description => "Calculates the factorial of a number";
        public string Category => "Number Tools";
        public string Execute(string input)
        {
            if (!int.TryParse(input, out int num) || num < 0)
            {
                return "Invalid input (must be a non-negative integer)";
            }

            if (num > 20) // Limit to avoid overflow
            {
                return "Input too large (max 20)";
            }

            long result = 1;
            for (int i = 2; i <= num; i++)
            {
                result *= i;
            }
            return result.ToString();
        }
    }
}
