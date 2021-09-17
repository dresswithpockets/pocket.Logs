using RabbitMQ.Client;

namespace pocket.Logs.Core.Interfaces
{
    public interface IQueueConnectionProvider
    {
        IConnection Connection { get; }

        bool EnsureConnection();
    }
}