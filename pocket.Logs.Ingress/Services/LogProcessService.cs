using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Options;

namespace pocket.Logs.Ingress.Services
{
    public class LogProcessService : CronJobService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<LogProcessService> _logger;

        public LogProcessService(IServiceScopeFactory serviceScopeFactory,
            IOptions<LogsTfProcessorConfiguration> configuration, ILogger<LogProcessService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            Expression = configuration.Value.Cron;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await DoWork(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var analysisService = scope.ServiceProvider.GetRequiredService<LogAnalysisService>();
            var logsContext = scope.ServiceProvider.GetRequiredService<LogsContext>();

            var logs = await logsContext.RetrievedLogs.OrderBy(l => l.CreatedAt).Where(l => !l.Processed)
                .ToListAsync(cancellationToken);
            _logger.LogInformation(IngressEventIds.ProcessingLogs,
                $"Dispatching {logs.Count} unprocessed logs to analysis service.");
            var tasks = logs.Select(l => analysisService.AnalyzeAsync(l, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }
}