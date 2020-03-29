using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Utils.SsoConfig
{
    public class IdSrvConfig
    {

        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Name = "portal",
                    Description = "Portal Application"
                },
                new Scope
                {
                    Name = "client",
                    Description = "client Application",
                    ScopeSecrets = new List<Secret>()
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference,
                    //Supaya tidak mencari file Allow Consent
                    RequireConsent = false,
                   //redirect Login setelah suckses login
                    RedirectUris =
                    {
                       "http://localhost:5001/signin-oidc"
                    },
                    //redirect sesudah logout
                    PostLogoutRedirectUris =
                    {
                        "http://localhost:5001"
                       //  Startup.GetSSO_Client()
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.OfflineAccess.Name,
                        "portal",
                        "client"
                    }
                }
            };
        }
    }
}
