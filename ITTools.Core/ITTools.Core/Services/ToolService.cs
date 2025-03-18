using ITTools.Core.Models;
using ITTools.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

namespace ITTools.Services
{
    public class ToolService
    {
        private readonly ApplicationDbContext _context;
        private readonly List<ITool> _pluginInstances;
        private readonly string _pluginPath;

        public ToolService(ApplicationDbContext context)
        {
            _context = context;
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
                            SyncToolWithDatabase(tool);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load {file}: {ex.Message}");
                }
            }

            lock (_pluginInstances)
            {
                _pluginInstances.Clear();
                _pluginInstances.AddRange(newPluginInstances);
            }
        }

        private void SyncToolWithDatabase(ITool tool)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Name == tool.Category);

            if (category == null)
            {
                category = new Category { Name = tool.Category };
                _context.Categories.Add(category);
                _context.SaveChanges();
            }

            var existingTool = _context.Tools
                .FirstOrDefault(t => t.Name == tool.Name);

            if (existingTool == null)
            {
                _context.Tools.Add(new Tool
                {
                    Name = tool.Name,
                    Description = tool.Description,
                    IsEnabled = true,
                    IsPremium = false, // Default value, can be changed by admin
                    CategoryId = category.Id
                });
                _context.SaveChanges();
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
    }
}