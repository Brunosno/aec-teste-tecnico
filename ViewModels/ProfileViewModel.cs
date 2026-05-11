using System.ComponentModel.DataAnnotations;

namespace AecTesteTecnico.ViewModels
{
    public class ProfileUpdateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido.")]
        public string? PhoneNumber { get; set; }

        [MinLength(6, ErrorMessage = "A senha deve possuir no mínimo 6 caracteres.")]
        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }
    }
}