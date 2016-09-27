using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;

namespace Patterns
{
    public class AuthenticationHandler
    {
        public void Handle(HttpRequestMessage request)
        {
            var scheme = request.Headers.Authorization?.Scheme;
            if (scheme == null)
            {
                throw new UnauthorizedAccessException();
            }

            var authenticationStrategy = GetAuthenticationStrategy(scheme);

            authenticationStrategy.Authenticate(request.Headers.Authorization.Parameter);
        }

        private IAuthenticationStrategy GetAuthenticationStrategy(string scheme)
        {
            var strategies = new Dictionary<string, Func<IAuthenticationStrategy>>()
            {
                {"Basic", () => new BasicAuthenticationStrategy()},
                {"JTokenDeflate", () => new IssuingAuthorityAuthenticationStrategy()},
                {"Ldap", () => new LdapAuthenticationStrategy()}
            };
            return strategies[scheme]();
        }
    }

    public interface IAuthenticationStrategy
    {
        void Authenticate(string authenticationHeaderValue);
    }

    public class BasicAuthenticationStrategy : IAuthenticationStrategy
    {
        public void Authenticate(string authenticationHeaderValue)
        {
            var credentials = ExtractCredentials(authenticationHeaderValue);
            if (VerifyCredentials(credentials))
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("put here data"), new []{"role"} );
            }
        }

        private ICredentials ExtractCredentials(string loginPassword)
        {
            throw new NotImplementedException();
        }

        private bool VerifyCredentials(ICredentials credentials)
        {
            throw new NotImplementedException();
        }
    }

    public class IssuingAuthorityAuthenticationStrategy : IAuthenticationStrategy
    {
        public void Authenticate(string authenticationHeaderValue)
        {
            var token = ParseToken(authenticationHeaderValue);
            var issuerUrl = GetIssuerUrl(token);
            VerifyAuthority(issuerUrl, token.Signature);
        }

        private class JToken
        {
            public string Signature { get; set; }
        }

        private JToken ParseToken(string token)
        {
            throw new NotImplementedException();
        }

        private Uri GetIssuerUrl(JToken jToken)
        {
            throw new NotImplementedException();
        }

        private void VerifyAuthority(Uri issuerUrl, string signature)
        {
            throw new NotImplementedException();
        }
    }

    public class LdapAuthenticationStrategy : IAuthenticationStrategy
    {
        public void Authenticate(string authenticationHeaderValue)
        {
            var endpoint = GetExternalProviderLdapEndpont();
            var connectionString = BuildConnectionString(endpoint, authenticationHeaderValue);
            RemoteAuthenticate(connectionString);
        }

        private Url GetExternalProviderLdapEndpont()
        {
            throw new NotImplementedException();
        }

        private string BuildConnectionString(Url externalProviderLdapEndpoint, string authenticationHeaderValue)
        {
            throw new NotImplementedException();
        }

        private void RemoteAuthenticate(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}