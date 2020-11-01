using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogistiqueApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel(opts =>
        {
            var configuration = (IConfiguration)opts.ApplicationServices.GetService(typeof(IConfiguration));
            opts.Listen(IPAddress.Loopback, 5000);
            opts.Listen(IPAddress.Loopback, 5001, listenOptions =>
            {
                listenOptions.UseHttps(
                    new X509Certificate2(configuration["certificates:ssl"],"")
                );
            });
        })
        .UseStartup<Startup>();
                });
    }
}
