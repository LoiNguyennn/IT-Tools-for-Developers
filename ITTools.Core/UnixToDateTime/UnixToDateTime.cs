using ITTools.Core.Models;
using System;

namespace ConversionTools
{
    public class UnixToDateTime : ITool
    {
        public string Name => "Unix To DateTime";
        public string Description => "Converts Unix timestamp to readable DateTime";
        public string Category => "Conversion Tools";

        public string Execute(string input)
        {
            if (!double.TryParse(input, out double unixTimestamp))
            {
                throw new ArgumentException("Input is not a valid Unix timestamp");
            }

            if (unixTimestamp < 0 || unixTimestamp > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Input is not a valid Unix timestamp");
            }

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}