using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NoWasteOfMoney.Infrastructure.Database;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Entities.NoWasteOfMoney.Domain.Entities;

namespace NoWasteOfMoney.Tests;

public class UsersControllerTests : IClassFixture<NoWasteOfMoneyWebApplicationFactory>
{
    private readonly NoWasteOfMoneyWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(NoWasteOfMoneyWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        _factory.SeedDatabase(db =>
        {
            if (db.Users.Any(u => u.Role == "User"))
            {
                return;
            }

            var regularPerson = new Person
            {
                Id = Guid.Parse("55555555-0000-0000-0000-000000000001"),
                FirstName = "Regular",
                LastName = "User",
                Email = "user@test.com"
            };

            var regularUser = new User
            {
                Id = Guid.Parse("66666666-0000-0000-0000-000000000001"),
                PersonId = regularPerson.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            db.Persons.Add(regularPerson);
            db.Users.Add(regularUser);
        });
    }

    [Fact]
    public async Task Create_AdminWithValidPayload_Returns201AndIncludesTemporaryPasswordAndToken()
    {
        // Arrange
        var token = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            name = "Novo Usuario",
            email = Guid.NewGuid().ToString() + "@test.com", // Email único por teste
            role = "User" // Note que removi a propriedade 'password'
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", payload);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var data = document.RootElement.GetProperty("data");

        Assert.Equal("Novo Usuario", data.GetProperty("name").GetString());
        Assert.Equal(payload.email, data.GetProperty("email").GetString());
        Assert.Equal("User", data.GetProperty("role").GetString());

        // Valida se as novas propriedades do fluxo de ativação existem e estão preenchidas
        var tempPassword = data.GetProperty("temporaryPassword").GetString();
        var activationToken = data.GetProperty("activationToken").GetString();

        Assert.NotNull(tempPassword);
        Assert.StartsWith("NWM@", tempPassword); // Garante o padrão que estipulamos
        Assert.NotNull(activationToken);
        Assert.True(Guid.TryParse(activationToken, out _)); // Verifica se o token é um Guid válido em string
    }
    [Fact]
    public async Task Login_WithTemporaryPassword_SucceedsAndFlagsRequiredReset()
    {
        // 1. Arrange: Criar o usuário primeiro como Admin para coletar a senha gerada
        var adminToken = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var targetEmail = Guid.NewGuid().ToString() + "@test.com";
        var creationPayload = new { name = "Usuario Temporario", email = targetEmail, role = "User" };

        var creationResponse = await _client.PostAsJsonAsync("/api/users", creationPayload);
        Assert.Equal(HttpStatusCode.Created, creationResponse.StatusCode);


        var responseWrapper = await creationResponse.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        string generatedTempPassword;


        if (responseWrapper != null && responseWrapper.ContainsKey("data"))
        {
            var dataElement = responseWrapper["data"];

            generatedTempPassword = dataElement.GetProperty("temporaryPassword").GetString()
                ?? dataElement.GetProperty("TemporaryPassword").GetString()!;
        }
        else
        {

            var rootElement = await creationResponse.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            generatedTempPassword = rootElement["temporaryPassword"].GetString()
                ?? rootElement["TemporaryPassword"].GetString()!;
        }

        _client.DefaultRequestHeaders.Authorization = null;

        var loginPayload = new
        {
            email = targetEmail,
            password = generatedTempPassword
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginPayload);

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        using var loginDoc = JsonDocument.Parse(loginBody);

        // 1. Primeiro pegamos a raiz do objeto retornado
        var root = loginDoc.RootElement;

        // 2. Navegamos para dentro do objeto "data" (Conforme o seu JSON real)
        var dataSection = root.GetProperty("data");

        // 3. Agora sim, extraímos as propriedades de dentro de "data" usando camelCase
        string accessToken = dataSection.GetProperty("accessToken").GetString()!;
        bool isPasswordResetRequired = dataSection.GetProperty("isPasswordResetRequired").GetBoolean();

        // 4. Validações finais do xUnit
        Assert.NotNull(accessToken);
        Assert.True(isPasswordResetRequired, "O teste falhou porque o usuário com senha temporária deveria exigir o reset de senha (true).");
    }

    [Fact]
    public async Task Create_NonAdminAuthenticated_Returns403()
    {
        var token = GenerateToken(
            role: "User",
            userId: Guid.Parse("66666666-0000-0000-0000-000000000001"),
            personId: Guid.Parse("55555555-0000-0000-0000-000000000001"),
            name: "Regular",
            email: "user@test.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            name = "Outro Usuario",
            email = Guid.NewGuid().ToString() + "@test.com",
            role = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/users", payload);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_Unauthenticated_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var payload = new
        {
            name = "Sem Auth",
            email = Guid.NewGuid().ToString() + "@test.com",
            role = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/users", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_AdminWithInvalidPayload_Returns400()
    {
        var token = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            name = "",
            email = "email-invalido",
            role = ""
        };

        var response = await _client.PostAsJsonAsync("/api/users", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_AdminWithEmailDuplicate_Returns409Conflict()
    {
        var token = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            name = "Novo Usuario",
            email = "user@test.com",
            role = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/users", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("E-mail já cadastrado", body);
    }

    [Fact]
    public async Task Create_AdminWithInvalidRole_Returns400BadRequest()
    {
        var token = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            name = "Novo Usuario",
            email = Guid.NewGuid().ToString() + "@test.com",
            role = "superadmin"
        };

        var response = await _client.PostAsJsonAsync("/api/users", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.Equal("Invalid role: superadmin", document.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Create_AdminWithValidRoles_Returns201()
    {
        var token = GenerateToken(role: "Admin", userId: DatabaseContext.SeedUserId, personId: DatabaseContext.SeedPersonId, name: "Pessoa", email: "adimin@semPerdaDeDinheiro.com");

        foreach (var validRole in new[] { "Admin", "User" })
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                name = $"Novo Usuario {validRole}",
                email = $"{Guid.NewGuid()}@test.com",
                role = validRole
            };

            var response = await _client.PostAsJsonAsync("/api/users", payload);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }

    private string GenerateToken(string role, Guid userId, Guid personId, string name, string email)
    {
        using var scope = _factory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey not configured"));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("PersonId", personId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = configuration["JwtSettings:Issuer"],
            Audience = configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}