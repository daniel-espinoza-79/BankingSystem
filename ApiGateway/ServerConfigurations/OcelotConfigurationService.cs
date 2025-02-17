using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiGateway.ServerConfigurations
{
    public static class OcelotConfigurationService
    {
        public const string ConfigDirectory = "RoutingConfiguration";
        public const string GlobalConfigFilePath = "RoutingConfiguration/ocelot.global.json";
        public const string OutputFilePath = "RoutingConfiguration/ocelot.json";
        public static void CombineConfigurations()
        {
            JObject? globalConfig = GetGlobalConfig();
            JArray allRoutes = LoadRoutes();
            JObject combinedConfig = CombineConfigurations(allRoutes, globalConfig);

            File.WriteAllText(OutputFilePath, combinedConfig.ToString(Formatting.Indented));
        }


        private static JObject? GetGlobalConfig()
        {
            if (File.Exists(GlobalConfigFilePath))
            {
                var globalJsonContent = File.ReadAllText(GlobalConfigFilePath);
                return JObject.Parse(globalJsonContent);
            }
            return null;
        }

        private static JArray LoadRoutes()
        {
            var allRoutes = new JArray();
            foreach (var filePath in Directory.GetFiles(ConfigDirectory, "ocelot.*.json", SearchOption.TopDirectoryOnly))
            {
                if (filePath == GlobalConfigFilePath)
                    continue;
                var serviceJsonContent = File.ReadAllText(filePath);
                var serviceRoutes = JArray.Parse(serviceJsonContent);

                foreach (var route in serviceRoutes)
                {
                    allRoutes.Add(route);
                }
            }
            return allRoutes;
        }

        private static JObject CombineConfigurations(JArray allRoutes, JObject? globalConfig)
        {
            var combinedConfig = new JObject();
            if (globalConfig != null)
            {
                combinedConfig["GlobalConfiguration"] = globalConfig["GlobalConfiguration"];
            }
            combinedConfig["Routes"] = allRoutes;
            return combinedConfig;
        }
    }
}