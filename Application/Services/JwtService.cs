using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HcAgents.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HcAgents.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var allClaims = new List<Claim>();
        allClaims.Add(
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            )
        );
        allClaims.Add(
            new Claim(
                JwtRegisteredClaimNames.Exp,
                DateTimeOffset
                    .UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationMinutes"]))
                    .ToUnixTimeSeconds()
                    .ToString(),
                ClaimValueTypes.Integer64
            )
        );
        allClaims.Add(
            new Claim(
                JwtRegisteredClaimNames.Nbf,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            )
        );
        allClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId, ClaimValueTypes.String));
        allClaims.Add(new Claim(JwtRegisteredClaimNames.Email, email, ClaimValueTypes.String));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(allClaims),
            Expires = DateTimeOffset
                .UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationMinutes"]))
                .DateTime,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
