using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MultiTenancy
{
    public class JwtHelper
    {
        public const string ISSUER = "https://localhost:7060";
        public const string AUDIENCE = "https://localhost:7060";
        public const string SECURITY_KEY = "tokenSecurityKey@1111fdccddssaassdyKey@1111fdccddssaassd11fdccdds";

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = ISSUER,
                ValidAudience = AUDIENCE,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
            };
        }

        public static string GenerateToken(AppUser user, List<Roles> roles,AppTenant tenant)
        {
            
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY));
            var signinCred = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);
            var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new(ClaimConstants.TenantId, tenant.Name ?? string.Empty)
            };
            if (roles != null)
            {
                
                foreach (var r in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r.Name));
                }
            }
            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: ISSUER,
                audience: AUDIENCE,
                signingCredentials: signinCred
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}
