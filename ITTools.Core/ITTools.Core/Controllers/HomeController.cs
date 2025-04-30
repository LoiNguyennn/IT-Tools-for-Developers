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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
    }
}
