﻿namespace ITTools.Core.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ToolId { get; set; }
        public Tool Tool { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}