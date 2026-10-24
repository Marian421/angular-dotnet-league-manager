using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class RefreshTokenDto
    {
        [Required] public string RefreshToken { get; set; }
    }
}
