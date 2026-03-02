using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using NoWasteOfMoney.Models.Entities.NoWasteOfMoney.Domain.Entities;


public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Buscando a SecretKey do appsettings.json
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("Secret Key não encontrada no appsettings.json"));

        var expiresAt = DateTime.UtcNow.AddHours(2); // Tempo de expiração padrão

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Person.FirstName), // Pega da entidade relacionada
                new Claim(ClaimTypes.Email, user.Person.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("PersonId", user.PersonId.ToString())
            }),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),

            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiresAt);
    }
}