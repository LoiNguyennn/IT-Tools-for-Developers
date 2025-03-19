using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberPlugins
{
    public class FibonacciGenerator : ITool
    {
        public string Name => "Fibonacci Generator";
        public string Description => "Generates a Fibonacci sequence up to n";
        public string Category => "Number Tools";
        public string Execute(string input)
        {
            if (!int.TryParse(input, out int n) || n <= 0)
            {
                return "Invalid input (must be a positive integer)";
            }

            if (n > 46) // Limit to avoid overflow with long
            {
                return "Input too large (max 46)";
            }

            var fib = new List<long> { 0, 1 };
            for (int i = 2; i < n; i++)
            {
                fib.Add(fib[i - 1] + fib[i - 2]);
            }
            return string.Join(", ", fib);
        }
    }
}
