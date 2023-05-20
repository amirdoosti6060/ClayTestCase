using Ocelot.Configuration;
using Ocelot.Middleware;
using System.Net;
using System.Security.Claims;

namespace ApiGateway.Helper
{
    public static class OcelotJwtMiddleware
    {
        private static readonly string RoleSeparator = ",";
        
        private static string? GetJwtRole(IEnumerable<Claim> claims)
        {
            string? role = null;

            Claim[] jwtClaims = null!;
            if (claims != null)
            {
                jwtClaims = claims.ToArray<Claim>();
                foreach(var claim in jwtClaims)
                {
                    if (claim.Type == "Role")
                        role = claim.Value;
                }
            }

            return role;
        }

        private static List<string> GetOcelotRoles(DownstreamRoute downstreamRoute)
        {
            List<string> roles = new List<string>();

            if (downstreamRoute != null)
            {
                var claims = downstreamRoute.RouteClaimsRequirement;
                string strRoles;

                claims.TryGetValue("Role", out strRoles!);
                if (strRoles != null)
                    roles = strRoles.Split(RoleSeparator, StringSplitOptions.TrimEntries).ToList(); ;
            }

            return roles!;
        }

        private static bool HasRole(string? jwtRole, List<string> ocelotRoles)
        {
            return ocelotRoles.Contains(jwtRole);
        }

        public static Func<HttpContext, Func<Task>, Task> 
            CreateAuthorizationFilter => async (httpContext, next) =>
            {
                var jwtRole = GetJwtRole(httpContext.User.Claims);
                var ocelotRoles = GetOcelotRoles((DownstreamRoute)httpContext.Items["DownstreamRoute"]!);

                if (ocelotRoles.Count != 0 && (jwtRole == null || !HasRole(jwtRole, ocelotRoles)))
                {
                    httpContext.Items.SetError(new UnauthenticatedError("Unauthorized!"));
                    return;
                }

                await next.Invoke();
            };
    }
}
