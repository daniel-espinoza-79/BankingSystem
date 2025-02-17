using Microsoft.OpenApi.Models;
using Ocelot.Middleware;

namespace ApiGateway.ServerConfigurations
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("auth", new OpenApiInfo { Title = "Authentication Service", Version = "v1" });
                c.SwaggerDoc("bank", new OpenApiInfo { Title = "Banking System", Version = "v1" });
                c.SwaggerDoc("oauth", new OpenApiInfo { Title = "OAuth Service", Version = "v1" });
            });
        }

        public static void UseSwaggerConfig(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("http://localhost:5000/swagger/authentication_service", "Authentication Service");
                c.SwaggerEndpoint("http://localhost:5000/swagger/banking_system", "Banking System");
                c.SwaggerEndpoint("http://localhost:5000/swagger/oauth_service", "OAuth Service");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
