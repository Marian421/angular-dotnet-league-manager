using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace backend.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int Id
        {
            get
            {
                var sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!int.TryParse(sub, out var id))
                {
                    throw new InvalidOperationException("There is no authenticated user for this request");
                }

                return id;
            }
        }
    }
}
