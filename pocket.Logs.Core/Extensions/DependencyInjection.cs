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
        public static IServiceCollection AddLogsConfiguration(this IServiceCollection serviceCollection,
            IConfiguration configuration) =>
            serviceCollection.Configure<LogsDbConfiguration>(configuration.GetSection(LogsDbConfiguration.LogsDb))
                .Configure<LogsTfIngressConfiguration>(
                    configuration.GetSection(LogsTfIngressConfiguration.LogsTfIngress))
                .Configure<LogsTfProcessorConfiguration>(
                    configuration.GetSection(LogsTfProcessorConfiguration.LogsTfProcessor))
                .Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.RabbitMq));
        
        public static IServiceCollection AddLogsDb(this IServiceCollection serviceCollection,
            IHostEnvironment hostEnvironment, IConfiguration configuration) => 
            serviceCollection.AddDbContext<LogsContext>(
            (s, b) =>
                b.UseLazyLoadingProxies()
                    .UseNpgsql(hostEnvironment.IsProduction()
                            ? s.GetRequiredService<IOptions<LogsDbConfiguration>>().Value.ConnectionString
                            : configuration.GetConnectionString("Default"),
                        x =>
                            x.MigrationsAssembly("pocket.Logs.Migrations")));
    }
}