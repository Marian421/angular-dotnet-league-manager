using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class RegisterUserDTO
    {
        [Required] public string Name { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] [MinLength(8)] public string Password { get; set; }
    }
}