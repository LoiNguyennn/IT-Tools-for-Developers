using ITTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimePlugins
{
    public class TimeZoneConverter : ITool
    {
        public string Name => "Time Zone Converter";
        public string Description => "Converts time between two time zones (input as 'time, fromZone, toZone')";
        public string Category => "Date/Time Tools";
        public string Execute(string input)
        {
            try
            {
                var parts = input.Split(',');
                if (parts.Length != 3)
                {
                    return "Invalid input: use 'time, fromZone, toZone' (e.g., '2025-03-18 14:00, UTC, PST')";
                }

                string timeStr = parts[0].Trim();
                string fromZoneStr = parts[1].Trim();
                string toZoneStr = parts[2].Trim();

                // Parse the input time
                if (!DateTime.TryParse(timeStr, out DateTime time))
                {
                    return "Invalid time format";
                }

                // Find time zones (Windows IDs like "Pacific Standard Time" or IANA like "America/Los_Angeles")
                TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById(fromZoneStr);
                TimeZoneInfo toZone = TimeZoneInfo.FindSystemTimeZoneById(toZoneStr);

                // Assume time is in fromZone, convert to UTC, then to toZone
                DateTimeOffset fromDateTime = new DateTimeOffset(time, fromZone.GetUtcOffset(time));
                DateTimeOffset toDateTime = TimeZoneInfo.ConvertTime(fromDateTime, toZone);

                return toDateTime.ToString("yyyy-MM-dd HH:mm");
            }
            catch (TimeZoneNotFoundException)
            {
                return "Invalid time zone ID";
            }
            catch
            {
                return "Error converting time";
            }
        }
    }
}
