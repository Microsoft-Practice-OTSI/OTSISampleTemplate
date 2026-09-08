using OTSISampleTemplate.Services.Models;

namespace OTSISampleTemplate.Services.Abstractions;

public interface IAuthService
{
    Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    string GenerateJwtToken(UserModel user);
}
