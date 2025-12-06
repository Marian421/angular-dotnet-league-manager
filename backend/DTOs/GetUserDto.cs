namespace backend.DTOs;

public class GetUserDto
{
    // ca exemplu
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
