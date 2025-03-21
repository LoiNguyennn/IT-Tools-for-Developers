using ITTools.Services;
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
            var tools = await _toolService.GetAllToolsAsync();
            return View(tools);
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string toolName, string input)
        {
            // Try to find the tool in plugin instances
            var pluginTool = _toolService.GetTools().FirstOrDefault(t => t.Name == toolName);
            if (pluginTool != null)
            {
                var result = pluginTool.Execute(input);
                return Json(new { result });
            }

            // If not found in plugins, check database for existence
            var tools = await _toolService.GetAllToolsAsync();
            var dbTool = tools.FirstOrDefault(t => t.Name == toolName);

            if (dbTool == null)
                return NotFound();

            // Return a message for non-executable tools
            return Json(new { result = $"Tool '{toolName}' is not executable (no plugin available)." });
        }
    }
}
