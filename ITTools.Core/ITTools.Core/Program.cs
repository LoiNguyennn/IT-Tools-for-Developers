using ITTools.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ITToolsDbContext>(options =>
    options.UseSqlite("Data Source=ITTools.db"));

// Danh sách plugin được load
var pluginInstances = new List<ITool>();

// Thư mục chứa plugin
string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);

void LoadPlugins()
{

    var newPluginInstances = new List<ITool>();

    foreach (var file in Directory.GetFiles(pluginPath, "*.dll"))
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

                var tool = Activator.CreateInstance(type) as ITool;
                if (tool != null)
                {
                    Console.WriteLine($"✅ Loaded tool: {tool.Name}");
                    newPluginInstances.Add(tool);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to load {file}: {ex.Message}");
        }
    }

    lock (pluginInstances)
    {
        pluginInstances.Clear();
        pluginInstances.AddRange(newPluginInstances);
    }

}



// Load plugin lần đầu
LoadPlugins();

// Đăng ký danh sách plugin với DI container
builder.Services.AddSingleton(serviceProvider => pluginInstances);


var app = builder.Build();

// Khởi tạo FileSystemWatcher để theo dõi thư mục Plugins
var watcher = new FileSystemWatcher(pluginPath, "*.dll")
{
    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
    EnableRaisingEvents = true
};

// Sử dụng Task.Run để tránh deadlock
watcher.Changed += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });
watcher.Created += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });
watcher.Deleted += (s, e) => Task.Run(() => { Thread.Sleep(1000); LoadPlugins(); });

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
