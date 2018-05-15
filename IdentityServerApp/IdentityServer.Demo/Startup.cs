using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Demo.AppCode;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Demo {
    public class Startup {
        
        public IConfiguration Configuration { get; }

        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {

            // services.AddCors ();

            services.AddMvc ();

            //Configuring IdentityServer4
            services.AddIdentityServer ()
                .AddDeveloperSigningCredential ()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources (Config.GetApiResources ())
                .AddInMemoryClients (Config.GetClients ())
                .AddTestUsers (Config.GetUsers ());

            services.AddMvcCore ()
                .AddAuthorization ()
                .AddJsonFormatters ();

            // // Adding Authentication services and configure "Bearer" as the default scheme.
            // services.AddAuthentication ("Bearer")
            //     .AddIdentityServerAuthentication (options => {
            //         options.Authority = "http://localhost:5000";
            //         options.RequireHttpsMetadata = false;
            //         options.ApiName = "api1";
            //     });

            // services.AddIdentityServer ();

            //Adding external authentication
            services.AddAuthentication ()
                // //Adding Google as an external authentication
                // .AddGoogle ("Google", options => {
                //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                //     options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
                //     options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
                // })
                //Adding OpenID Connect
                .AddOpenIdConnect ("demoidsrv", "IdentityServer", options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "implicit";
                    options.ResponseType = "id_token";
                    options.SaveTokens = true;
                    options.CallbackPath = new PathString ("/signin-idsrv");
                    options.SignedOutCallbackPath = new PathString ("/signout-callback-idsrv");
                    options.RemoteSignOutPath = new PathString ("/signout-idsrv");

                    options.TokenValidationParameters = new TokenValidationParameters {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Home/Error");
            }

            // app.UseCors (builder => {
            //     builder.AllowAnyOrigin ()
            //         .AllowAnyMethod ()
            //         .AllowAnyHeader ()
            //         .AllowCredentials ();
            // });

            //Adding IdentityServer4 to the pipeline
            app.UseIdentityServer ();

            //Adding the Authentication middleware to the pipeline
            app.UseAuthentication ();

            app.UseStaticFiles ();

            //Adding MVC routing
            app.UseMvcWithDefaultRoute ();
        }
    }
}