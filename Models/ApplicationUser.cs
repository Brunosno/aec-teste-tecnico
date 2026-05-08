using Microsoft.AspNetCore.Identity;
namespace AecTesteTecnico.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public Perfil Perfil { get; set; }

    public List<Address> Addresses { get; set; } = new();

    public string? ImageUrl { get; set; }
}