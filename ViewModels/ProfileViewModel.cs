namespace AecTesteTecnico.ViewModels
{
    public class ProfileUpdateDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }
    }
}