using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Extensions;
using pocket.Logs.Core.Options;

namespace pocket.Logs.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            // ensure the db can be connected to
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<LogsContext>();
                var connection = db?.Database.GetDbConnection();
                connection.Open();
                connection.Close();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(b => b.AddDatabaseUrl(LogsDbConfiguration.LogsDb))
                .ConfigureLogging(l => l.AddConsole())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}