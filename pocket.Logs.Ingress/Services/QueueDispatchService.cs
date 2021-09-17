using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pocket.Logs.Core.Interfaces;
using pocket.Logs.Ingress.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace pocket.Logs.Ingress.Services
{
    public class QueueDispatchService : BackgroundService
    {
        private readonly IQueueConnectionProvider _queueConnectionProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public QueueDispatchService(IQueueConnectionProvider queueConnectionProvider,
            IServiceScopeFactory serviceScopeFactory)
        {
            _queueConnectionProvider = queueConnectionProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = _queueConnectionProvider.Connection.CreateModel();
            channel.DeclareGeneralQueues();

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += OnConsumerReceived;
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            channel.BasicConsume(consumer, Queues.LogsIngress, true);
            // get next thing in queue, check what kind of queue item it is (i.e log to be processed, player refresh
            // request, etc), and dispatch to the correct handler.
        }

        private async Task OnConsumerReceived(object model, BasicDeliverEventArgs e)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var analysisService = scope.ServiceProvider.GetRequiredService<LogAnalysisService>();
            
            var body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var logs = JsonSerializer.Deserialize<int[]>(json);

            /*await Task.WhenAll(logs.Select(l => analysisService.AnalyzeFromQuery(l)));*/
        }

        private async Task OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private async Task OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private async Task OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private async Task OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
    }
}