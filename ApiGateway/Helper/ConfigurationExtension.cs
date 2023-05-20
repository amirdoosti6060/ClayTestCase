using Ocelot.Configuration.File;

namespace ApiGateway.Helper
{
    public class GlobalRole : Dictionary<string, string> { 
    }

    public static class ConfigurationExtension
    {
        public static IServiceCollection ConfigureResolveRolePlaceholders(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.PostConfigure<FileConfiguration>(fileConfiguration =>
            {
                var globalRoles = configuration
                    .GetSection($"Authorization")
                    .Get<GlobalRole>();

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureAuthorization(route, globalRoles!);
                }
            });

            return services;
        }

        private static void ConfigureAuthorization(FileRoute route, GlobalRole globalRoles)
        {
            string roleVar, role, placeholder;


            foreach (var item in route.RouteClaimsRequirement.Keys.ToList())
            {
                roleVar = route.RouteClaimsRequirement[item];

                if (roleVar.StartsWith("{") && roleVar.EndsWith("}"))
                {
                    placeholder = roleVar.TrimStart('{').TrimEnd('}');
                    if (globalRoles.TryGetValue(placeholder, out role))
                    {
                        route.RouteClaimsRequirement[item] = role;
                    }
                }
            }
        }
    }
}
