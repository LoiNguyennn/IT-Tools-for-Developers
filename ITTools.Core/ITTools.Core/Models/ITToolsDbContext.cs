using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ITTools.Core.Models
{
    public class ITToolsDbContext : DbContext
    {
        public ITToolsDbContext(DbContextOptions<ITToolsDbContext> options) : base(options) { }

        public DbSet<Tool> Tools { get; set; }
    }
}