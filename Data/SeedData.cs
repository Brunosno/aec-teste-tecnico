using Microsoft.AspNetCore.Identity;
using AecTesteTecnico.Data;
using AecTesteTecnico.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("ADMIN"))
            await roleManager.CreateAsync(new IdentityRole("ADMIN"));

        if (!await roleManager.RoleExistsAsync("CLIENT"))
            await roleManager.CreateAsync(new IdentityRole("CLIENT"));

        var adminEmail = "admin@admin.com";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Administrador",
                Perfil = Perfil.ADMIN
            };

            var result = await userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "ADMIN");
            }
        }
    }
}