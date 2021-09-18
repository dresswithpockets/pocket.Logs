using System;
using Microsoft.Extensions.Options;
using pocket.Logs.Core.Interfaces;
using pocket.Logs.Core.Options;
using RabbitMQ.Client;

namespace pocket.Logs.Ingress.Services
{
    public class QueueConnectionProvider : IQueueConnectionProvider
    {
        private readonly string _hostname;
        private readonly int _port;
        private readonly string _password;
        private readonly string _username;
        private IConnection? _connection;

        public IConnection Connection
        {
            get
            {
                if (!EnsureConnection() || _connection == null) throw new InvalidOperationException();
                return _connection;
            }
        }

        public QueueConnectionProvider(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _port = rabbitMqOptions.Value.Port;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
        }

        public bool EnsureConnection()
        {
            if (_connection?.IsOpen ?? false) return true;
            CreateConnection();
            return _connection?.IsOpen ?? false;
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    Port = _port,
                    UserName = _username,
                    Password = _password,
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }
    }
}