namespace AecTesteTecnico.Models;

public class Address
{
    public int Id { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string? Complement { get; set; }

    public string Neighborhood { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public int Number { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;
}