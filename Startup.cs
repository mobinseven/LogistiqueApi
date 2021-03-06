using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Python.Runtime;
using System;
using System.IO;

namespace LogistiqueApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
            services.AddMemoryCache();
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            services.AddIdentityServer(options =>
                        {
                            options.IssuerUri = configuration["IdentityServer:Issuer"];
                        })
                    .LoadSigningCredentialFrom(configuration["certificates:signing"])
                    .AddInMemoryApiResources(LogistiqueAuthority.Data.ResourceManager.Apis)
                    .AddInMemoryClients(LogistiqueAuthority.Data.ClientManager.Clients);
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = configuration["IdentityServer:Authority"];
                        options.ApiName = "logistique.api";
                        options.RequireHttpsMetadata = Convert.ToBoolean(configuration["IdentityServer:RequireHttpsMetadata"]);
                    });

            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // services.AddHttpsRedirection(options =>
            // {
            //     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //     options.HttpsPort = Convert.ToInt32(configuration["HttpsPort"]);
            // });
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
