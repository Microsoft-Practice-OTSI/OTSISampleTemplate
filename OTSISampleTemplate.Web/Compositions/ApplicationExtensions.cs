using OTSISampleTemplate.Services;
using OTSISampleTemplate.Services.Abstractions;

namespace OTSISampleTemplate.Web.Compositions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();
    }
}
