using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityAjaxClient
{
    public static class JwtHelper
    {
        public static IEnumerable<Claim> DecodeJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims;
        }
    }
}
