namespace backend.Services
{
    public interface IPasswordService
    {
        string HashPassword(Models.User user, string password);
        bool VerifyPassword(Models.User user, string hashedPassword, string providedPassword);
        
    }
}