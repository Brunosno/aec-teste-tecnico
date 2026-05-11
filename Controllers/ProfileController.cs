using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AecTesteTecnico.Models;
using AecTesteTecnico.Services;
using AecTesteTecnico.ViewModels;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly MinioService _minio;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        MinioService minio,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _minio = minio;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var dto = new ProfileUpdateDTO
        {
            Name = user!.Name,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            ImageUrl = user.ImageUrl
        };

        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            dto.ImageUrl =
                await _minio.GetPresignedUrlAsync(dto.ImageUrl);
        }

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProfileUpdateDTO dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View("Index", dto);

        user.Name = dto.Name;

        user.PhoneNumber = dto.PhoneNumber;

        if (user.Email != dto.Email)
        {
            var emailExists =
                await _userManager.FindByEmailAsync(dto.Email);

            if (emailExists != null &&
                emailExists.Id != user.Id)
            {
                ModelState.AddModelError(
                    "Email",
                    "Este email já está em uso."
                );

                return View("Index", dto);
            }

            var emailResult =
                await _userManager.SetEmailAsync(user, dto.Email);

            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description
                    );
                }

                return View("Index", dto);
            }

            user.UserName = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                ModelState.AddModelError(
                    "ConfirmPassword",
                    "As senhas não coincidem."
                );

                return View("Index", dto);
            }

            var samePassword =
                await _userManager.CheckPasswordAsync(
                    user,
                    dto.NewPassword
                );

            if (samePassword)
            {
                ModelState.AddModelError(
                    "NewPassword",
                    "A nova senha não pode ser igual à senha atual."
                );

                return View("Index", dto);
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResult =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    dto.NewPassword
                );

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description
                    );
                }

                return View("Index", dto);
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        if (dto.Image != null && dto.Image.Length > 0)
        {
            int imageIndex = 1;

            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                var name =
                    user.ImageUrl.Split('/').Last();

                var match =
                    System.Text.RegularExpressions.Regex.Match(
                        name,
                        @"img(\d+)"
                    );

                if (match.Success)
                {
                    imageIndex =
                        int.Parse(match.Groups[1].Value) + 1;
                }

                await _minio.DeleteAsync(user.ImageUrl);
            }

            user.ImageUrl =
                await _minio.UploadAsync(
                    dto.Image,
                    "users",
                    user.UserName!,
                    user.Id,
                    imageIndex
                );
        }

        var updateResult =
            await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description
                );
            }

            return View("Index", dto);
        }

        return RedirectToAction(nameof(Index));
    }
}