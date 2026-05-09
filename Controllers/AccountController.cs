using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AecTesteTecnico.Models;
using AecTesteTecnico.ViewModels;
using AecTesteTecnico.Services;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly MinioService _minio;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             MinioService minio)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _minio = minio;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existingUser = await _userManager.FindByNameAsync(model.UserName);

        if (existingUser != null)
        {
            ModelState.AddModelError("UserName", "Este nome de usuário já existe.");
            return View(model);
        }

        var existingEmail = await _userManager.FindByEmailAsync(model.Email);

        if (existingEmail != null)
        {
            ModelState.AddModelError("Email", "Este email já está em uso.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            Name = model.Name,
            Perfil = Perfil.CLIENT
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "CLIENT");

            if (image != null && image.Length > 0)
            {
                user.ImageUrl = await _minio.UploadAsync(
                    image,
                    "users",
                    user.UserName,
                    user.Id,
                    1
                );

                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Profile");
        }

        ModelState.AddModelError("", "Usuário ou senha inválidos");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}