using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AecTesteTecnico.Data;
using AecTesteTecnico.Models;

[Authorize]
public class AddressController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public AddressController(
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var addresses = _context.Addresses
            .Where(a => a.UserId == user!.Id)
            .ToList();

        return View(addresses);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Address address)
    {
        if (!ModelState.IsValid)
            return View(address);

        var user = await _userManager.GetUserAsync(User);

        address.UserId = user!.Id;

        _context.Add(address);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var address = _context.Addresses
            .FirstOrDefault(a =>
                a.Id == id &&
                a.UserId == user!.Id);

        if (address != null)
        {
            _context.Remove(address);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}