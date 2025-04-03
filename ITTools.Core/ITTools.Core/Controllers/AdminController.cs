using ITTools.Core.Models;
using ITTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;

namespace ITTools.Core.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ToolService _toolService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ToolService toolService, IWebHostEnvironment webHostEnvironment)
        {
            _toolService = toolService;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> EditTool(int id, [Bind("Id,Name,Description,IsEnabled,IsPremium,CategoryId")] Tool tool, IFormFile IconFile)
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

                // Handle icon file upload if provided
                if (IconFile != null && IconFile.Length > 0)
                {
                    if (Path.GetExtension(IconFile.FileName).ToLower() != ".svg")
                    {
                        ModelState.AddModelError("IconFile", "Only SVG files are allowed.");
                        ViewBag.Categories = await _toolService.GetCategoriesAsync();
                        return View(tool);
                    }

                    // Get the path to save the icon
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // If the tool name has changed, delete the old icon file
                    if (existingTool.Name != tool.Name)
                    {
                        string oldIconPath = Path.Combine(uploadsFolder, $"{existingTool.Name.Replace(" ", "")}.svg");
                        if (System.IO.File.Exists(oldIconPath))
                        {
                            System.IO.File.Delete(oldIconPath);
                        }
                    }

                    // Save the new icon file
                    string iconFileName = $"{tool.Name.Replace(" ", "")}.svg";
                    string filePath = Path.Combine(uploadsFolder, iconFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await IconFile.CopyToAsync(fileStream);
                    }
                }
                else if (existingTool.Name != tool.Name)
                {
                    // If the tool name changed but no new icon uploaded, rename the existing icon
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets");
                    string oldIconPath = Path.Combine(uploadsFolder, $"{existingTool.Name.Replace(" ", "")}.svg");
                    string newIconPath = Path.Combine(uploadsFolder, $"{tool.Name.Replace(" ", "")}.svg");

                    if (System.IO.File.Exists(oldIconPath))
                    {
                        System.IO.File.Move(oldIconPath, newIconPath, true);
                    }
                }

                // Update tool properties
                existingTool.Name = tool.Name;
                existingTool.Description = tool.Description;
                existingTool.IsEnabled = tool.IsEnabled;
                existingTool.IsPremium = tool.IsPremium;
                existingTool.CategoryId = tool.CategoryId;

                await _toolService.UpdateToolAsync(existingTool);
                TempData["SuccessMessage"] = $"Tool '{tool.Name}' has been successfully updated.";
                return RedirectToAction("Index", "Tools");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _toolService.ToolExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating tool: {ex.Message}");
                ViewBag.Categories = await _toolService.GetCategoriesAsync();
                return View(tool);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTool(int id)
        {
            try
            {
                var tool = await _toolService.GetToolByIdAsync(id);
                if (tool == null)
                {
                    return NotFound();
                }

                // Delete the icon file if it exists
                string iconFileName = $"{tool.Name.Replace(" ", "")}.svg";
                string iconPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", iconFileName);
                if (System.IO.File.Exists(iconPath))
                {
                    System.IO.File.Delete(iconPath);
                }

                await _toolService.DeleteToolAsync(id);
                _toolService.LoadPlugins();
                TempData["SuccessMessage"] = $"Tool '{tool.Name}' has been successfully deleted.";
                return RedirectToAction("Index", "Tools");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting tool: {ex.Message}";
                return RedirectToAction("EditTool", new { id });
            }
        }

 
        [HttpGet]
        public IActionResult AddTool()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTool(IFormFile dllFile)
        {
            if (dllFile == null || dllFile.Length == 0)
            {
                ModelState.AddModelError("dllFile", "Please select a DLL file");
                return View();
            }

            if (!dllFile.FileName.EndsWith(".dll"))
            {
                ModelState.AddModelError("dllFile", "Please select a valid DLL file");
                return View();
            }

            try
            {
                string pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

                // Ensure the Plugins directory exists
                if (!Directory.Exists(pluginsPath))
                {
                    Directory.CreateDirectory(pluginsPath);
                }

                string filePath = Path.Combine(pluginsPath, dllFile.FileName);

                if (System.IO.File.Exists(filePath))
                {
                    ModelState.AddModelError("dllFile", "A plugin with this name already exists");
                    return View();
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dllFile.CopyToAsync(stream);
                }

                _toolService.LoadPlugins();
                TempData["Success"] = "Tool uploaded successfully! It will be available shortly.";

                // Instead of redirecting immediately, return to the view to show the modal
                // The JavaScript will handle the redirect after the modal is dismissed
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error uploading file: {ex.Message}");
                return View();
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}