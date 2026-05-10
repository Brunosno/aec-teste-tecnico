using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using AecTesteTecnico.Data;
using AecTesteTecnico.Models;
using AecTesteTecnico.ViewModels;

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
        return View(new AddressViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddressViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);

        var address = new Address
        {
            ZipCode = model.ZipCode,
            Street = model.Street,
            Number = model.Number,
            Complement = model.Complement,
            Neighborhood = model.Neighborhood,
            City = model.City,
            Uf = model.Uf,

            UserId = user!.Id
        };

        _context.Add(address);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var address = _context.Addresses
            .FirstOrDefault(a =>
                a.Id == id &&
                a.UserId == user!.Id);

        if (address == null)
            return NotFound();

        var model = new AddressViewModel
        {
            ZipCode = address.ZipCode,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            Uf = address.Uf
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        AddressViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);

        var address = _context.Addresses
            .FirstOrDefault(a =>
                a.Id == id &&
                a.UserId == user!.Id);

        if (address == null)
            return NotFound();

        address.ZipCode = model.ZipCode;
        address.Street = model.Street;
        address.Number = model.Number;
        address.Complement = model.Complement;
        address.Neighborhood = model.Neighborhood;
        address.City = model.City;
        address.Uf = model.Uf;

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

    [HttpGet]
    public async Task<IActionResult> ExportCsv()
    {
        var user = await _userManager.GetUserAsync(User);

        var addresses = _context.Addresses
            .Where(a => a.UserId == user!.Id)
            .ToList();

        var builder = new StringBuilder();

        builder.AppendLine("Id,Street,City,Uf,ZipCode");

        foreach (var address in addresses)
        {
            builder.AppendLine($"{address.Id}," +
                            $"{Escape(address.Street)}," +
                            $"{Escape(address.City)}," +
                            $"{Escape(address.Uf)}," +
                            $"{Escape(address.ZipCode)}");
        }

        var fileName = $"enderecos_{user.UserName}_{DateTime.Now:yyyyMMddHHmmss}.csv";
        var bytes = Encoding.UTF8.GetBytes(builder.ToString());

        return File(bytes, "text/csv", fileName);
    }

    private string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ExportCsvs()
    {
        var addresses = _context.Addresses
            .Select(a => new
            {
                a.Id,
                a.Street,
                a.Number,
                a.Complement,
                a.Neighborhood,
                a.City,
                a.Uf,
                a.ZipCode,

                UserName = a.User!.UserName,
                UserEmail = a.User.Email
            })
            .ToList();

        var builder = new StringBuilder();

        builder.AppendLine(
            "Id,Usuario,Email,Rua,Numero,Complemento,Bairro,Cidade,UF,CEP"
        );

        foreach (var address in addresses)
        {
            builder.AppendLine(
                $"{address.Id}," +
                $"{Escape(address.UserName)}," +
                $"{Escape(address.UserEmail)}," +
                $"{Escape(address.Street)}," +
                $"{address.Number}," +
                $"{Escape(address.Complement)}," +
                $"{Escape(address.Neighborhood)}," +
                $"{Escape(address.City)}," +
                $"{Escape(address.Uf)}," +
                $"{Escape(address.ZipCode)}"
            );
        }

        var fileName =
            $"enderecos_geral_{DateTime.Now:yyyyMMddHHmmss}.csv";

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());

        return File(
            bytes,
            "text/csv",
            fileName
        );
    }
}