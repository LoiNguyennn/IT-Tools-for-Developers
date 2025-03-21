using ITTools.Core.Models;
using ITTools.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;
using System.Linq;

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
            var loadedToolNames = new HashSet<string>();

            // Tải tất cả các plugin từ thư mục Plugins
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
                            loadedToolNames.Add(tool.Name); // Theo dõi tên công cụ đã tải
                            SyncToolWithDatabase(tool);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load {file}: {ex.Message}");
                }
            }

            // Xóa các công cụ trong database không còn tương ứng với plugin
            var toolsToRemove = _context.Tools
                .Where(t => !loadedToolNames.Contains(t.Name))
                .ToList();

            if (toolsToRemove.Any())
            {
                _context.Tools.RemoveRange(toolsToRemove);
                _context.SaveChanges();
                foreach (var tool in toolsToRemove)
                {
                    Console.WriteLine($"🗑️ Removed tool from database: {tool.Name}");
                }
            }

            // Cập nhật danh sách plugin trong bộ nhớ
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
                    IsPremium = false,
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

        public async Task<List<Tool>> GetAllToolsAsync()
        {
            return await _context.Tools.ToListAsync();
        }

        public async Task<Tool?> GetToolByIdAsync(int id)
        {
            return await _context.Tools
                .Include(t => t.Category) // Include Category for display
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task UpdateToolAsync(Tool tool)
        {
            _context.Update(tool);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ToolExistsAsync(int id)
        {
            return await _context.Tools.AnyAsync(t => t.Id == id);
        }
    }
}