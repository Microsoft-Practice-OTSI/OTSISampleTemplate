using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OTSISampleTemplate.Web.Compositions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddCorsPolicy(this WebApplicationBuilder builder, string policyName)
    {
        var corsBuilder = new CorsPolicyBuilder()
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowAnyOrigin();
        builder.Services.AddCors(options => { options.AddPolicy(policyName, corsBuilder.Build()); });
    }

    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"] ?? "OTSISampleTemplateSecretKeyWithMinimum256BitsLength1234567890!";
        var issuer = jwtSettings["Issuer"] ?? "OTSISampleTemplate";
        var audience = jwtSettings["Audience"] ?? "OTSISampleTemplateAudience";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Check Authorization header first; fallback to cookie
                    if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("access_token"))
                    {
                        context.Token = context.Request.Cookies["access_token"];
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // For browser requests (HTML), redirect to Login page
                    if (context.Request.Headers.Accept.ToString().Contains("text/html"))
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Account/Login");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorization();
    }
}
