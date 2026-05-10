using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AecTesteTecnico.Models;

[Authorize(Roles = "ADMIN")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Manage(string? search)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower().Trim();

            query = query.Where(u =>
                u.UserName!.ToLower().Contains(search) ||
                u.Email!.ToLower().Contains(search));
        }

        var users = await query
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var result = new List<(ApplicationUser User, IList<string> Roles)>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add((user, roles));
        }

        ViewBag.Search = search ?? string.Empty;

        return View(result);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        ViewBag.Roles = roles;

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var isAdmin = await _userManager.IsInRoleAsync(user, "ADMIN");

        if (isAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, "ADMIN");
        }
        else
        {
            if (!await _roleManager.RoleExistsAsync("ADMIN"))
            {
                await _roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }

            await _userManager.AddToRoleAsync(user, "ADMIN");
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
        {
            user.LockoutEnd = null;
        }
        else
        {
            user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
        }

        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Manage));
    }
}