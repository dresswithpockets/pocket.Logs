using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Interfaces;
using pocket.Logs.Ingress.Options;
using pocket.Logs.Ingress.Services;

namespace pocket.Logs.Ingress
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LogsDbConfiguration>(Configuration.GetSection(LogsDbConfiguration.LogsDb));
            services.Configure<LogsTfIngressConfiguration>(
                Configuration.GetSection(LogsTfIngressConfiguration.LogsTfIngress));
            services.Configure<LogsTfProcessorConfiguration>(
                Configuration.GetSection(LogsTfProcessorConfiguration.LogsTfProcessor));
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection(RabbitMqConfiguration.RabbitMq));

            services.AddDbContext<LogsContext>((s, b) => b.UseLazyLoadingProxies().UseNpgsql(
                HostEnvironment.IsProduction()
                    ? s.GetRequiredService<IOptions<LogsDbConfiguration>>().Value.ConnectionString
                    : Configuration.GetConnectionString("Default"),
                x => x.MigrationsAssembly("pocket.Logs.Migrations")));

            services.AddGrpc();

            services.AddFluentValidation(c =>
                c.RegisterValidatorsFromAssemblyContaining<Startup>(includeInternalTypes: true));

            services.AddHttpClient();
            //services.AddTransient<IQueueConnectionProvider, QueueConnectionProvider>();
            services.AddTransient<ILogClient, LogClient>();
            services.AddScoped<LogAnalysisService>();

            services.AddHostedService<LogProcessService>();
            services.AddHostedService<IngressService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }
}