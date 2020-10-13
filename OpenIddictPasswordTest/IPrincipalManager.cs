using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;

namespace OpenIddictPasswordTest
{
    public interface IPrincipalManager
    {
        ClaimsPrincipal Create(string sub, IEnumerable<string> scopes);
    }

    public class ScopePrincipalManager : IPrincipalManager
    {
        public ClaimsPrincipal Create(string sub, IEnumerable<string> scopes)
        {
            var identity = Principals.ShellIdentity;

            foreach (var scope in scopes)
            {
                identity.AddClaim(new Claim("scope", scope).SetDestinations(new[] { ".access_token" }));
            }

            return new ClaimsPrincipal(identity);
        }
    }

    public class OIScpPrincipalManager : IPrincipalManager
    {
        public ClaimsPrincipal Create(string sub, IEnumerable<string> scopes)
        {
            var identity = Principals.ShellIdentity;

            foreach (var scope in scopes)
            {
                identity.AddClaim(new Claim("oi_scp", scope).SetDestinations(new[] { ".access_token" }));
            }

            return new ClaimsPrincipal(identity);
        }
    }

    public static class Principals
    {
        public static ClaimsIdentity ShellIdentity
        {
            get
            {
                var identity = new ClaimsIdentity("Bearer", ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim("sub", "TEST").SetDestinations(new[] { ".access_token" }));

                return identity;
            }
        }
    }
}
