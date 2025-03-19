using ITTools.Core.Models;

namespace DateTimePlugins
{
    public class CurrentTimestamp : ITool
    {
        public string Name => "Current Timestamp";
        public string Description => "Returns the current Unix timestamp";
        public string Category => "Date/Time Tools";
        public string Execute(string input)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }
    }
}
