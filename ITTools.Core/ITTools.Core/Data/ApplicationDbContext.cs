using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ITTools.Core.Models;

namespace ITTools.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tool> Tools { get; set; } = null!;
        public DbSet<Favorite> Favorites { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for Favorite
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.ToolId });

            // Configure relationship: Favorite -> ApplicationUser (many-to-one)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure relationship: Favorite -> Tool (many-to-one)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Tool)
                .WithMany(t => t.Favorites)
                .HasForeignKey(f => f.ToolId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure relationship: Tool -> Category (many-to-one)
            modelBuilder.Entity<Tool>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tools)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull) // Nếu Category bị xóa, CategoryId thành null
                .IsRequired(false);              // Category là tùy chọn

            // Additional configurations for better performance and consistency
            modelBuilder.Entity<Tool>()
                .HasIndex(t => t.CategoryId);

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => f.ToolId);

            // Property configurations
            modelBuilder.Entity<Tool>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Tool>()
                .Property(t => t.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}