using ITTools.Core.Models;
using ITTools.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ITTools.Core.Controllers
{
    public class AdminController : Controller
    {
        private readonly ToolService _toolService;
        public AdminController(ToolService toolService) {
            _toolService = toolService;
        }

        [HttpGet]
        public async Task<IActionResult> EditTool(int id)
        {
            var tool = await _toolService.GetToolByIdAsync(id);
            if (tool == null)
            {
                return NotFound();
            }

            // Pass categories for dropdown
            ViewBag.Categories = await _toolService.GetCategoriesAsync();

            return View(tool);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTool(int id, [Bind("Id,Name,Description,IsEnabled,IsPremium,CategoryId")] Tool tool)
        {
            if (id != tool.Id)
            {
                return BadRequest();
            }
           
            try
            {
                var existingTool = await _toolService.GetToolByIdAsync(id);
                if (existingTool == null)
                {
                    return NotFound();
                }

                existingTool.Name = tool.Name;
                existingTool.Description = tool.Description;
                existingTool.IsEnabled = tool.IsEnabled;
                existingTool.IsPremium = tool.IsPremium;
                existingTool.CategoryId = tool.CategoryId;

                await _toolService.UpdateToolAsync(existingTool);
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _toolService.ToolExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Categories = await _toolService.GetCategoriesAsync();
                return View(tool);
            }

            ViewBag.Categories = await _toolService.GetCategoriesAsync();
            return View(tool);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
