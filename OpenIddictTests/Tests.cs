using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenIddictPasswordTest;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OpenIddictTests
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [Theory]
        [InlineData(typeof(OIScpPrincipalManager), "openid")]
        [InlineData(typeof(OIScpPrincipalManager), "openid email")]
        [InlineData(typeof(OIScpPrincipalManager), "openid email profile")]
        [InlineData(typeof(OIScpPrincipalManager), "openid email profile offline_access")]
        [InlineData(typeof(OIScpPrincipalManager), "openid email profile offline_access test1")]

        [InlineData(typeof(ScopePrincipalManager), "openid")]
        [InlineData(typeof(ScopePrincipalManager), "openid email")]
        [InlineData(typeof(ScopePrincipalManager), "openid email profile")]
        [InlineData(typeof(ScopePrincipalManager), "openid email profile offline_access")]
        [InlineData(typeof(ScopePrincipalManager), "openid email profile offline_access test1")]
        public async Task ScopeClaimContainsAllRequestedScopes(Type principalManager, string scope)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrincipalManager));

                    services.Remove(descriptor);

                    services.AddSingleton(typeof(IPrincipalManager), principalManager);
                });
            })
            .CreateClient();

            var response = await client.PostAsync("/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "id" },
                { "client_secret", "secret" },
                { "username", "user" },
                { "password", "pass" },
                { "scope", scope },
            }));

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(await response.Content.ReadAsStringAsync());

            accessTokenResponse.AccessToken.Should().NotBeNullOrWhiteSpace();

            var token = new JwtSecurityTokenHandler().ReadJwtToken(accessTokenResponse.AccessToken);

            token.Claims.First(c => c.Type == "scope").Value.Should().Be(scope);
        }

        [Theory]
        [InlineData(typeof(OIScpPrincipalManager))]
        [InlineData(typeof(ScopePrincipalManager))]
        public async Task ResponseContainsIDToken(Type principalManager)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrincipalManager));

                    services.Remove(descriptor);

                    services.AddSingleton(typeof(IPrincipalManager), principalManager);
                });
            })
            .CreateClient();

            var response = await client.PostAsync("/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "id" },
                { "client_secret", "secret" },
                { "username", "user" },
                { "password", "pass" },
                { "scope", "openid" },
            }));

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(await response.Content.ReadAsStringAsync());

            accessTokenResponse.IdToken.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(OIScpPrincipalManager))]
        [InlineData(typeof(ScopePrincipalManager))]
        public async Task ResponseContainsRefreshToken(Type principalManager)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrincipalManager));

                    services.Remove(descriptor);

                    services.AddSingleton(typeof(IPrincipalManager), principalManager);
                });
            })
            .CreateClient();

            var response = await client.PostAsync("/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "id" },
                { "client_secret", "secret" },
                { "username", "user" },
                { "password", "pass" },
                { "scope", "offline_access" },
            }));

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(await response.Content.ReadAsStringAsync());

            accessTokenResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        private class AccessTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("id_token")]
            public string IdToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }
        }
    }
}
