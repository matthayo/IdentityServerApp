
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServerApp.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IdentityServer4;

using IdentityServerApp.Identity.AppCode;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServerApp.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Configure identity server with in-memory stores, keys, clients and scopes 
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<IdentityUser>();

            services.AddMvcCore ()
                .AddAuthorization ()
                .AddJsonFormatters ();
            
            //Adding external authentication
            services.AddAuthentication ()
                //Adding Google as an external authentication
                .AddGoogle ("Google", options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                    options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                })
                //Adding OpenID Connect
                .AddOpenIdConnect ("oidc", "OpenID Connect", options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "http://localhost:5002/";
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
                    options.RequireHttpsMetadata = false;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // app.UseAuthentication();
            app.UseIdentityServer();

            //Adding MVC routing
            app.UseMvcWithDefaultRoute ();
        }
    }
}
