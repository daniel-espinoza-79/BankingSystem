using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.ServerConfigurations;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        string authority = configuration["Authority"] ?? throw new Exception("Authority not found in configuration");
        string audience = configuration["Audience"] ?? throw new Exception("Audience not found in configuration");
        services.AddAuthorization();
        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        )
        .AddJwtBearer(
            options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true
                };
            }
        );
    }
}