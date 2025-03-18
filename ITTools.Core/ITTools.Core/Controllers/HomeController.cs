using ITTools.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITTools.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITToolsDbContext _dbContext;

        public HomeController(IServiceProvider serviceProvider, ITToolsDbContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            Console.WriteLine("🏠 HomeController - Index action called");

            // Lấy danh sách plugin mới nhất từ DI container
            var tools = _serviceProvider.GetRequiredService<List<ITool>>();

            var dbTools = _dbContext.Tools.ToList();
            var enabledTools = tools
                .Where(t => dbTools.Any(db => db.Name == t.Name && db.IsEnabled))
                .ToList();

            // Cập nhật DB nếu có tool mới
            foreach (var tool in tools.Where(t => !dbTools.Any(db => db.Name == t.Name)))
            {
                _dbContext.Tools.Add(new Tool
                {
                    Name = tool.Name,
                    Description = tool.Description,
                    IsEnabled = true,
                    IsPremium = false
                });
            }
            _dbContext.SaveChanges();

            Console.WriteLine($"📊 Enabled tools count: {enabledTools.Count}");

            return View(enabledTools);
        }

        [HttpPost]
        public IActionResult Execute(string toolName, string input)
        {
            Console.WriteLine($"⚡ Executing tool: {toolName}");

            var tools = _serviceProvider.GetRequiredService<List<ITool>>();
            var tool = tools.FirstOrDefault(t => t.Name == toolName);

            if (tool == null || !_dbContext.Tools.Any(t => t.Name == toolName && t.IsEnabled))
            {
                Console.WriteLine("❌ Tool not found or disabled");
                return Json(new { success = false, message = "Tool not found or disabled" });
            }

            var result = tool.Execute(input);
            Console.WriteLine($"✅ Execution result: {result}");

            return Json(new { success = true, result });
        }
    }
}
