using Microsoft.Extensions.Logging;

namespace pocket.Logs.Ingress
{
    public class IngressEventIds
    {
        public static readonly EventId ProcessingLogs = new(0, nameof(ProcessingLogs));
    }
}