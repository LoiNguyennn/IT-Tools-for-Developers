using ITTools.Core.Models;
using System;

namespace TokenGenerators
{
    public class UUIDV4Generator : ITool
    {
        public string Name => "UUID V4 Generator";
        public string Description => "Generates a random UUID version 4";
        public string Category => "Token Generation Tools";

        public string Execute(string input)
        {
            // Generate a random UUID v4 using Guid.NewGuid() and format it to match RFC 4122
            Guid guid = Guid.NewGuid();
            return guid.ToString("D"); // "D" format ensures hyphens (e.g., 550e8400-e29b-41d4-a716-446655440000)
        }
    }
}