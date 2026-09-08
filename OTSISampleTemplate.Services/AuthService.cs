using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OTSISampleTemplate.Services.Abstractions;
using OTSISampleTemplate.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OTSISampleTemplate.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    // In-memory demo users
    private static readonly List<(string Username, string Password, UserModel User)> DemoUsers =
    [
        ("admin", "Admin@123", new UserModel
        {
            Id = "USR-001",
            Username = "admin",
            FullName = "Administrator",
            Email = "admin@otsi.co",
            Role = "Admin"
        }),
        ("john.doe", "User@123", new UserModel
        {
            Id = "USR-002",
            Username = "john.doe",
            FullName = "John Doe",
            Email = "john.doe@otsi.co",
            Role = "User"
        })
    ];

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AuthResponse> AuthenticateAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Message = "Username and password are required."
            });
        }

        var matched = DemoUsers.FirstOrDefault(u =>
            string.Equals(u.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.Password == request.Password);

        if (matched == default)
        {
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Message = "Invalid username or password. Please try again."
            });
        }

        var token = GenerateJwtToken(matched.User);
        var expirySetting = _configuration["Jwt:ExpiryInMinutes"];
        var expiryMinutes = int.TryParse(expirySetting, out var parsed) ? parsed : 60;

        return Task.FromResult(new AuthResponse
        {
            Success = true,
            Message = "Authentication successful.",
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = matched.User
        });
    }

    public string GenerateJwtToken(UserModel user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "OTSISampleTemplateSecretKeyWithMinimum256BitsLength1234567890!";
        var issuer = _configuration["Jwt:Issuer"] ?? "OTSISampleTemplate";
        var audience = _configuration["Jwt:Audience"] ?? "OTSISampleTemplateAudience";
        var expirySetting = _configuration["Jwt:ExpiryInMinutes"];
        var expiryMinutes = int.TryParse(expirySetting, out var parsed) ? parsed : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
