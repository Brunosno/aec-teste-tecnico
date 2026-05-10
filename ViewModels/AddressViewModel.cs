using System.ComponentModel.DataAnnotations;

namespace AecTesteTecnico.ViewModels;

public class AddressViewModel
{
    [Required(ErrorMessage = "CEP é obrigatório")]
    [MaxLength(9)]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rua é obrigatória")]
    public string Street { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Número inválido")]
    public int Number { get; set; }

    public string? Complement { get; set; }

    [Required(ErrorMessage = "Bairro é obrigatório")]
    public string Neighborhood { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cidade é obrigatória")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "UF é obrigatória")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "UF deve ter 2 letras")]
    public string Uf { get; set; } = string.Empty;
}