using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Extensions;
using pocket.Logs.Core.Options;

namespace pocket.Logs.Ingress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            { 
                var config = scope.ServiceProvider.GetService<IOptions<LogsDbConfiguration>>();
                throw new Exception(config.Value.ConnectionString);

                var db = scope.ServiceProvider.GetService<LogsContext>();
                db?.Database?.Migrate();
            }

            host.Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(b => b.AddDatabaseUrl(LogsDbConfiguration.LogsDb))
                .ConfigureLogging(l => l.AddConsole())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}