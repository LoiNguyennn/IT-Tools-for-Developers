using ITTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ITTools.Core.Models;
using ITTools.Data;

namespace ITTools.Core.Controllers
{
    public class ToolsController : Controller
    {
        private readonly ToolService _toolService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ToolsController(ToolService toolService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _toolService = toolService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(bool myTools = false)
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _toolService.GetAllToolsAsync());
            }
            
            var tools = await _toolService.GetEnabledToolsAsync();
            
            if (!myTools)
                return View(tools);
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            tools = tools
                .Where(t => t.Favorites.Any(f => f.UserId == user.Id))
                .ToList();

            return View(tools);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tool = await _toolService.GetToolByIdAsync(id);
            if (tool == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(User);
            ViewBag.IsPremium = user?.IsPremium ?? false;

            var pluginTool = _toolService.GetTools().FirstOrDefault(t => t.Name == tool.Name);
            ViewBag.HasPlugin = pluginTool != null;

            return View(tool);
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string toolName, string input)
        {
            var pluginTool = _toolService.GetTools()
                .FirstOrDefault(t => t.Name == toolName);
            
            if (pluginTool != null)
            {
                var result = pluginTool.Execute(input);
                return Json(new { result });
            }

            var tools = await _toolService.GetAllToolsAsync();
            var dbTool = tools.FirstOrDefault(t => t.Name == toolName);

            if (dbTool == null)
                return NotFound();

            return Json(new { result = $"Tool '{toolName}' is not executable (no plugin available)." });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite([FromForm] int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            
            var existing = _context.Favorites
                .FirstOrDefault(f => f.ToolId == id && f.UserId == user.Id);

            var type = 0;
            if (existing != null)
            {
                _context.Favorites.Remove(existing);
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    ToolId = id,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                });
                type = 1;
            }
            
            await _context.SaveChangesAsync();

            return Json(new { success = true, type });
        }
    }
}
