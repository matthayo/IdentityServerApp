using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer.Demo.AppCode {
    public class Config {
        //Defining the API
        public static IEnumerable<ApiResource> GetApiResources () 
        {
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
                            }
            };
        }
        public static List<TestUser> GetUsers () {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                new TestUser {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
    }
}