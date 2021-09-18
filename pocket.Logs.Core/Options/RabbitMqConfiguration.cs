﻿namespace pocket.Logs.Core.Options
{
    public class RabbitMqConfiguration
    {
        public const string RabbitMq = "RabbitMq";
        
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}