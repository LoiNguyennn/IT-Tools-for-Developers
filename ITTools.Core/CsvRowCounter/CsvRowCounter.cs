using ITTools.Core.Models;

namespace FileFormat
{
    public class CSVRowCounter : ITool
    {
        public string Name => "CSV Row Counter";
        public string Description => "Counts rows in a CSV string";
        public string Category => "File Format Tools";
        public string Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            // Split by newline and remove empty lines
            var rows = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return rows.Length.ToString();
        }
    }
}
