using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Server;

namespace OpenIddictPasswordTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddAuthorization();

            services.AddSingleton<IPrincipalManager, OIScpPrincipalManager>();

            services
                .AddOpenIddict()
                .AddServer(options =>
                {
                    options
                        .UseAspNetCore()
                        .DisableTransportSecurityRequirement();

                    options
                        .EnableDegradedMode()
                        .DisableAccessTokenEncryption()
                        .DisableScopeValidation()
                        .SetTokenEndpointUris("/token")
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options
                        .AddEventHandler<OpenIddictServerEvents.ValidateTokenRequestContext>(options => options.UseScopedHandler<ValidateTokenRequestHandler>())
                        .AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(options => options.UseScopedHandler<TokenRequestHandler>());
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
