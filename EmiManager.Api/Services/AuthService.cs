using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EmiManager.Api.Services;
public class AuthService {
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public SymmetricSecurityKey IssuerSigningKey { get; init; }

    public AuthService(string issuer, string audience, string key) {
        Issuer = issuer;
        Audience = audience;

        var encodedBytes = Encoding.UTF8.GetBytes(key);
        IssuerSigningKey = new SymmetricSecurityKey(encodedBytes);
    }

    public string SignTokenForUser(string email, string name) {
        Claim[] claims = new Claim[] {
            new (ClaimTypes.Email, email),
            new(ClaimTypes.Name, name)
        };
        ClaimsIdentity subject = new(claims);
        
        SecurityTokenDescriptor descriptor = new() {
            Subject = subject,
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha512Signature)
        };

        JwtSecurityTokenHandler handler = new();
        SecurityToken token = handler.CreateToken(descriptor);
        string jwtToken = handler.WriteToken(token);
        return jwtToken;
    }
}
