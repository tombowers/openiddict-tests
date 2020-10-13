using OpenIddict.Server;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace OpenIddictPasswordTest
{
    public class ValidateTokenRequestHandler : IOpenIddictServerHandler<ValidateTokenRequestContext>
    {
        public ValueTask HandleAsync(ValidateTokenRequestContext context)
            => default;
    }
}
