using ITTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITTools.Core.Controllers
{
    public class ToolsController : Controller
    {
        private readonly ToolService _toolService;

        public ToolsController(ToolService toolService)
        {
            _toolService = toolService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _toolService.GetAllToolsAsync());
            }
            var tools = await _toolService.GetEnabledToolsAsync();
            return View(tools);

        }

        public async Task<IActionResult> Details(int id)
        {
            var tool = await _toolService.GetToolByIdAsync(id);
            if (tool == null)
            {
                return NotFound();
            }

            var pluginTool = _toolService.GetTools().FirstOrDefault(t => t.Name == tool.Name);
            ViewBag.HasPlugin = pluginTool != null;

            return View(tool);
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string toolName, string input)
        {
            var pluginTool = _toolService.GetTools().FirstOrDefault(t => t.Name == toolName);
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


       
    }
}
