﻿namespace ITTools.Core.Models
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }
        string Category { get; } 
        string Execute(string input);
    }
}