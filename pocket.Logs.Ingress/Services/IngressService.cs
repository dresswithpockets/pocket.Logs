using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Interfaces;
using pocket.Logs.Ingress.Extensions;

namespace pocket.Logs.Ingress.Services
{
    public class IngressService : CronJobService
    {
        private readonly IQueueConnectionProvider _queueConnectionProvider;
        private readonly ILogClient _logClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly uint _bulkLimit;
        private readonly uint _queryLimit;

        public IngressService(IQueueConnectionProvider queueConnectionProvider, ILogClient logClient,
            IServiceScopeFactory serviceScopeFactory, IOptions<LogsTfIngressConfiguration> configuration)
        {
            _queueConnectionProvider = queueConnectionProvider;
            _logClient = logClient;
            _serviceScopeFactory = serviceScopeFactory;

            _bulkLimit = configuration.Value.BulkLimit;
            _queryLimit = configuration.Value.QueryLimit;
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
            var context = scope.ServiceProvider.GetRequiredService<LogsContext>();
            
            // get our most recently retrieved log ID
            var mostRecent = await context.RetrievedLogs.OrderByDescending(r => r.LogId)
                .FirstOrDefaultAsync(cancellationToken);
            var mostRecentId = mostRecent?.LogId;
            
            // get the most recent n logs from logs.tf - if our stored log ID isn't in the list, continue to aggregate
            // more logs
            var logs = new List<QueryLog>();
            var offset = 0U;

            do
            {
                try
                {
                    var query = await _logClient.GetLogs(limit: _queryLimit, offset: offset);
                    logs.AddRange(query.Logs);
                }
                catch (HttpRequestException)
                {
                    continue;
                }

                offset += _queryLimit;
            } while (mostRecentId == null
                ? logs.Count >= _bulkLimit
                : !logs.Select(l => l.Id).Contains(mostRecentId.Value));

            // add the new log information to the database, only adding the latest logs which have not already been
            // included - this assumes that the log Ids are monotonic (can never go backwards)
            var uniqueLogs = logs.Where(l => l.Id > mostRecentId);
            var transformedLogs = uniqueLogs.Select(l => new RetrievedLog
            {
                LogId = l.Id,
                Title = l.Title,
                Map = l.Map,
                CreatedAt = l.Date,
                RetrievedAt = DateTime.UtcNow,
                Views = l.Views,
                Players = l.Players,
                Processed = false,
            }).ToList();
            await context.AddRangeAsync(transformedLogs, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
            /*var unprocessedLogs = await context.RetrievedLogs.Where(r => !r.Processed).Select(r => r.LogId)
                .ToListAsync(cancellationToken);

            // add the logs to be processed to the queue
            using var channel = _queueConnectionProvider.Connection.CreateModel();
            channel.DeclareGeneralQueues();

            var json = JsonSerializer.Serialize(unprocessedLogs);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(string.Empty, Queues.LogsIngress, true, null, body);*/
        }
    }
}