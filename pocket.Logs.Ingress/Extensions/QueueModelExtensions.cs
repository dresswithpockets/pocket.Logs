using System.Collections.Generic;
using RabbitMQ.Client;

namespace pocket.Logs.Ingress.Extensions
{
    public static class QueueModelExtensions
    {
        public static IEnumerable<QueueDeclareOk> DeclareGeneralQueues(this IModel model)
        {
            var oks = new List<QueueDeclareOk> { model.QueueDeclare(Queues.LogsIngress, true, false, false, null) };
            return oks;
        }
    }
}