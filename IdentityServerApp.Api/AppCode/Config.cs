using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerApp.Api.AppCode {
    public class Config {
        //Defining the API
        public static IEnumerable<ApiResource> GetApiResources () {
            return new List<ApiResource> {
                new ApiResource ("api1", "My API")
            };
        }
        //Defining the client
        public static IEnumerable<Client> GetClients () {
            return new List<Client> {
                new Client {
                    ClientId = "client",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // secret for authentication
                        ClientSecrets = {
                            new Secret ("secret".Sha256 ())
                        },
                        // scopes that client has access to
                        AllowedScopes = { "api1" }
                },

                //Resource Owner password grant client
                new Client {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets = {
                        new Secret ("secret".Sha256 ())
                    },
                    AllowedScopes = { "api1" }
                },

                // // OpenID Connect implicit flow client (MVC)
                // new Client {
                //     ClientId = "mvc",
                //     ClientName = "MVC Client",
                //     AllowedGrantTypes = GrantTypes.Implicit,

                //     // where to redirect to after login
                //     RedirectUris = { "http://localhost:5002/signin-oidc" },

                //     // where to redirect to after logout
                //     PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                //     AllowedScopes = new List<string> {
                //         IdentityServerConstants.StandardScopes.OpenId,
                //         IdentityServerConstants.StandardScopes.Profile
                //     }
                // },
                
                // OpenID Connect Hybrid Flow client
                new Client{
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string> {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
        public static List<TestUser> GetUsers () {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",
                    Claims = new []
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }

        //Helper method to create a collection of IdentityResource objects.
        public static IEnumerable<IdentityResource> GetIdentityResources () {
            return new List<IdentityResource> {
                new IdentityResources.OpenId (),
                new IdentityResources.Profile (),
            };
        }
    }
}