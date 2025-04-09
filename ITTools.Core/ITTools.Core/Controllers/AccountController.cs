using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ITTools.Core.Models;
using ITTools.Core.viewModels;
using ITTools.Data;
using Microsoft.EntityFrameworkCore;

namespace ITTools.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User"); // Default role
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Detail()
        {
            var user = await _userManager.GetUserAsync(User);
            
            var favoriteTools = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.CreatedAt)
                .Include(f => f.Tool)
                .ThenInclude(t => t.Category)
                .Select(f => f.Tool)
                .Take(3)
                .ToListAsync();

            var viewModel = new AccountDetailViewModel
            {
                User = user!,
                FavoriteTools = favoriteTools,
                IsPremium = user!.IsPremium
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public async Task<IActionResult> Upgrade()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.IsPremium)
            {
                TempData["ShowUpgradeAlert"] = true;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

       [HttpPost, ActionName("Upgrade")]
        public async Task<IActionResult> UpgradeUser()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return Unauthorized();
            }

            user.IsPremium = true;
            
            var result = await _userManager.UpdateAsync(user);

            return Json(result.Succeeded
                ? new { success = true, redirectUrl = Url.Action("Detail", "Account") }
                : new { success = false, errors = result.Errors.Select(e => e.Description) });
        }
    }
}