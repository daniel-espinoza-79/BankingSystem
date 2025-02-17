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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication Service", Version = "v1" });
            });
        }

        public static void UseSwaggerConfig(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("http://localhost:5000/swagger/authentication_service", "Authentication Service");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
