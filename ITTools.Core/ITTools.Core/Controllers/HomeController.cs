using ITTools.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITTools.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToolService _toolService;
        public HomeController(ToolService toolService)
        {
            _toolService = toolService;
        }

        public async Task<IActionResult> Index()
        {
            var tools = await _toolService.GetAllToolsAsync();
            return View(tools);
        }
    }
}
