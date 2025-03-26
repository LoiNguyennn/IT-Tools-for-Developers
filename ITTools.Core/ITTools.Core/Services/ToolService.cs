using ITTools.Core.Models;
using ITTools.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Add this for IServiceScopeFactory
using System.Reflection;
using System.IO;
using System.Runtime.Loader;
using System.Linq;
using System.Threading.Tasks;

namespace ITTools.Services
{
    public class ToolService
    {
        private readonly IServiceScopeFactory _scopeFactory; // Replace ApplicationDbContext with scope factory
        private readonly List<ITool> _pluginInstances;
        private readonly string _pluginPath;

        public ToolService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory; // Inject scope factory instead of DbContext
            _pluginInstances = new List<ITool>();
            _pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

            if (!Directory.Exists(_pluginPath))
                Directory.CreateDirectory(_pluginPath);

            LoadPlugins();
            SetupFileWatcher();
        }

        public IReadOnlyList<ITool> GetTools() => _pluginInstances.AsReadOnly();

        private void LoadPlugins()
        {
            var newPluginInstances = new List<ITool>();
            var loadedToolNames = new HashSet<string>();

            // Load all plugins from the Plugins directory
            foreach (var file in Directory.GetFiles(_pluginPath, "*.dll"))
            {
                try
                {
                    byte[] assemblyBytes = File.ReadAllBytes(file);
                    var context = new AssemblyLoadContext(null, true);
                    var assembly = context.LoadFromStream(new MemoryStream(assemblyBytes));

                    var toolTypes = assembly.GetTypes()
                        .Where(t => typeof(ITool).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in toolTypes)
                    {
                        if (Activator.CreateInstance(type) is ITool tool)
                        {
                            Console.WriteLine($"✅ Loaded tool: {tool.Name}");
                            newPluginInstances.Add(tool);
                            loadedToolNames.Add(tool.Name);
                            SyncToolWithDatabase(tool); // This will now use a scoped context
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load {file}: {ex.Message}");
                }
            }

            // Use a scope to clean up tools from the database
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var toolsToRemove = dbContext.Tools
                    .Where(t => !loadedToolNames.Contains(t.Name))
                    .ToList();

                if (toolsToRemove.Any())
                {
                    dbContext.Tools.RemoveRange(toolsToRemove);
                    dbContext.SaveChanges();
                    foreach (var tool in toolsToRemove)
                    {
                        Console.WriteLine($"🗑️ Removed tool from database: {tool.Name}");
                    }
                }
            }

            // Update the in-memory plugin list
            lock (_pluginInstances)
            {
                _pluginInstances.Clear();
                _pluginInstances.AddRange(newPluginInstances);
            }
        }

        private void SyncToolWithDatabase(ITool tool)
        {
            // Create a new scope for database operations
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var category = dbContext.Categories
                    .FirstOrDefault(c => c.Name == tool.Category);

                if (category == null)
                {
                    category = new Category { Name = tool.Category };
                    dbContext.Categories.Add(category);
                    dbContext.SaveChanges();
                }

                var existingTool = dbContext.Tools
                    .FirstOrDefault(t => t.Name == tool.Name);

                if (existingTool == null)
                {
                    dbContext.Tools.Add(new Tool
                    {
                        Name = tool.Name,
                        Description = tool.Description,
                        IsEnabled = true,
                        IsPremium = false,
                        CategoryId = category.Id
                    });
                    dbContext.SaveChanges();
                }
            }
        }

        private void SetupFileWatcher()
        {
            var watcher = new FileSystemWatcher(_pluginPath, "*.dll")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Changed += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });
            watcher.Created += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });
            watcher.Deleted += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });
        }

        public async Task<List<Tool>> GetAllToolsAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return await dbContext.Tools.ToListAsync();
            }
        }

        public async Task<Tool?> GetToolByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return await dbContext.Tools
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return await dbContext.Categories.ToListAsync();
            }
        }

        public async Task UpdateToolAsync(Tool tool)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Update(tool);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ToolExistsAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return await dbContext.Tools.AnyAsync(t => t.Id == id);
            }
        }
    }
}