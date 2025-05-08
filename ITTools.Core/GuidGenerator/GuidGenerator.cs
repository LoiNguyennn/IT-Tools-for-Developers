using ITTools.Core.Models;

namespace TokenGenerators
{
    public class GUIDGenerator : ITool
    {
        public string Name => "GUID Generator";
        public string Description => "Generates a unique identifier (GUID)";
        public string Category => "Token Generation Tools";

        public string Execute(string input)
        {
            return Guid.NewGuid().ToString();
        }
    }
}