using OpenIddict.Abstractions;
using OpenIddict.Server;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace OpenIddictPasswordTest
{
    public class TokenRequestHandler : IOpenIddictServerHandler<HandleTokenRequestContext>
    {
        private readonly IPrincipalManager _principalManager;

        public TokenRequestHandler(IPrincipalManager principalManager)
        {
            _principalManager = principalManager ?? throw new System.ArgumentNullException(nameof(principalManager));
        }

        public ValueTask HandleAsync(HandleTokenRequestContext context)
        {
            context.Principal = _principalManager.Create("TEST", context.Request.GetScopes());

            return default;
        }
    }
}