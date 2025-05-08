using ITTools.Core.Models;

namespace DateTimePlugins
{
    public class CurrentTimeStamp : ITool
    {
        public string Name => "Current TimeStamp";
        public string Description => "Returns the current Unix timestamp";
        public string Category => "Date/Time Tools";
        public string Execute(string input)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }
    }
}
