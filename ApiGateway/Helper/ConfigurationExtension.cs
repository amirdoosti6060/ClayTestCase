using Ocelot.Configuration.File;

namespace ApiGateway.Helper
{
    public class OcelotAuthorization : Dictionary<string, string> 
    { 
    }

    public class OcelotHosts : Dictionary<string, Uri>
    {
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
                    .GetSection($"OcelotParams:Authorization")
                    .Get<OcelotAuthorization>();

                var globalHosts = configuration
                    .GetSection($"OcelotParams:Hosts")
                    .Get<OcelotHosts>();

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureAuthorization(route, globalRoles!, globalHosts!);
                }
            });

            return services;
        }

        private static void ConfigureAuthorization(FileRoute route, 
            OcelotAuthorization ocelotAuthorization,
            OcelotHosts ocelotHosts)
        {
            string roleVar, param, placeholder;
            Uri uri;


            foreach (var item in route.RouteClaimsRequirement.Keys.ToList())
            {
                roleVar = route.RouteClaimsRequirement[item];

                if (roleVar.StartsWith("{") && roleVar.EndsWith("}"))
                {
                    placeholder = roleVar.TrimStart('{').TrimEnd('}');
                    if (ocelotAuthorization.TryGetValue(placeholder, out param))
                    {
                        route.RouteClaimsRequirement[item] = param;
                    }
                }
            }

            foreach (var hostPort in route.DownstreamHostAndPorts)
            {
                var host = hostPort.Host;

                if (host.StartsWith("{") && host.EndsWith("}"))
                {
                    placeholder = hostPort.Host.TrimStart('{').TrimEnd('}');
                    if (ocelotHosts.TryGetValue(placeholder, out uri))
                    {
                        route.DownstreamScheme = uri.Scheme;
                        hostPort.Host = uri.Host;
                        hostPort.Port = uri.Port;
                    }
                }
            }
        }
    }
}
