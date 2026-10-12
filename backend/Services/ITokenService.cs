namespace backend.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(Models.User user);
    }
}
