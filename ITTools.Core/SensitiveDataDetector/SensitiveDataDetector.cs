using ITTools.Core.Models;
using System.Text.RegularExpressions;

namespace SecurityTools
{
    public class SensitiveDataDetector : ITool
    {
        public string Name => "Sensitive Data Detector";

        public string Description =>
            "Scans strings for emails, tokens, passwords, and API keys. Flags suspicious data and suggests redactions.";

        public string Category => "Security Tools";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "No input provided.";

            var findings = new List<string>();

            // Email pattern
            var emailMatches = Regex.Matches(input, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            foreach (Match match in emailMatches)
                findings.Add($"Possible email: {match.Value} → Suggest: {Redact(match.Value)}");

            // API key / token pattern
            var apiKeyMatches = Regex.Matches(input, @"(?i)(api[_\-]?key|access[_\-]?token)[^=\n]*[=:]\s*[\""]?[\w\-]{16,}[\""]?");
            foreach (Match match in apiKeyMatches)
                findings.Add($"Possible API key/token: {Trim(match.Value)} → Suggest: {RedactValue(match.Value)}");

            // Secret / password pattern
            var secretMatches = Regex.Matches(input, @"(?i)(password|secret)[^=\n]*[=:]\s*[\""]?.{6,}[\""]?");
            foreach (Match match in secretMatches)
                findings.Add($"Possible secret: {Trim(match.Value)} → Suggest: {RedactValue(match.Value)}");

            // JWT-like pattern
            var jwtMatches = Regex.Matches(input, @"eyJ[\w\-]+\.[\w\-]+\.[\w\-]+");
            foreach (Match match in jwtMatches)
                findings.Add($"Possible JWT token: {match.Value} → Suggest: {Redact(match.Value)}");

            return findings.Count > 0
                ? string.Join("\n", findings)
                : "No sensitive data detected.";
        }

        private string Redact(string value)
        {
            return value.Length <= 8
                ? "[REDACTED]"
                : value.Substring(0, 4) + "..." + value.Substring(value.Length - 4);
        }

        private string Trim(string line)
        {
            return line.Length > 64 ? line.Substring(0, 64) + "..." : line;
        }

        private string RedactValue(string line)
        {
            // Redact everything after the = or :
            var split = Regex.Split(line, @"[=:]");
            if (split.Length < 2) return "[REDACTED]";
            var label = split[0].Trim();
            return $"{label} = [REDACTED]";
        }
    }
}