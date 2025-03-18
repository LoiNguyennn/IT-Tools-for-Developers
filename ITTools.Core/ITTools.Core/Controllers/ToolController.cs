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

        public IActionResult Index()
        {
            var tools = _toolService.GetTools();
            return View(tools);
        }

        [HttpPost]
        public IActionResult Execute(string toolName, string input)
        {
            var tool = _toolService.GetTools().FirstOrDefault(t => t.Name == toolName);
            if (tool == null)
                return NotFound();

            var result = tool.Execute(input);
            return Json(new { result });
        }
    }
}
