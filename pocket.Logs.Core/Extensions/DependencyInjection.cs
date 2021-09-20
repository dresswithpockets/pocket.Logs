using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Options;

namespace pocket.Logs.Core.Extensions
{
    public static class DependencyInjection
    {
        public static IConfigurationBuilder AddDatabaseUrl(this IConfigurationBuilder configurationBuilder, string path = "", string urlSource = "DATABASE_URL")
        {
            var config = configurationBuilder.Build();
            var dbUrl = config.GetValue<string>(urlSource);
            return configurationBuilder.Add(new DatabaseUrlConfigurationSource(dbUrl, path));
        }

        public static IServiceCollection AddLogsConfiguration(this IServiceCollection serviceCollection,
            IConfiguration configuration) =>
            serviceCollection.Configure<LogsDbConfiguration>(configuration.GetSection(LogsDbConfiguration.LogsDb));

        public static IServiceCollection AddLogsIngressConfiguration(this IServiceCollection serviceCollection,
            IConfiguration configuration) => 
            serviceCollection.Configure<LogsTfIngressConfiguration>(
                    configuration.GetSection(LogsTfIngressConfiguration.LogsTfIngress))
                .Configure<LogsTfProcessorConfiguration>(
                    configuration.GetSection(LogsTfProcessorConfiguration.LogsTfProcessor))
                .Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.RabbitMq));

        public static IServiceCollection AddLogsDb(this IServiceCollection serviceCollection,
            IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            return serviceCollection.AddDbContext<LogsContext>(
                (s, b) =>
                {
                    var config = s.GetRequiredService<IOptions<LogsDbConfiguration>>();
                    var connString = hostEnvironment.IsProduction()
                        ? config.Value.ConnectionString
                        : configuration.GetConnectionString("Default");
                    b.UseLazyLoadingProxies()
                        .UseNpgsql(connString,
                            x =>
                            {
                                x.MigrationsAssembly("pocket.Logs.Migrations");
                                
                                if (!config.Value.SslRequired || config.Value.CaCert == null) return;
                                
                                var bytes = Encoding.UTF8.GetBytes(config.Value.CaCert);
                                x.ProvideClientCertificatesCallback(c => c.Add(new X509Certificate(bytes)));
                            });
                });
        }
    }
}