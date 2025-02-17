using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;

namespace ApiGateway.ServerConfigurations;

public static class OcelotConfiguration
{
    public static void ConfigureOcelot(this IServiceCollection services, ConfigurationManager configuration)
    {
        OcelotConfigurationService.CombineConfigurations();
        configuration.AddJsonFile("RoutingConfiguration/ocelot.json", optional: false, reloadOnChange: true);
        services.AddOcelot(configuration)
            .AddCacheManager(settings => settings.WithDictionaryHandle());
    }
}