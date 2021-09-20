using System;
using System.Text.Json.Serialization;
using Npgsql;

namespace pocket.Logs.Core.Options
{
    public class LogsDbConfiguration
    {
        public const string LogsDb = "LogsDb";
        
        public string Host { get; set; }

        public int Port { get; set; }

        public string Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public SslMode? SslMode { get; set; }

        [JsonIgnore] public bool SslRequired => SslMode == Npgsql.SslMode.Require;

        public string CaCert { get; set; }

        [JsonIgnore]
        public string ConnectionString =>
            $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};{(SslMode != null ? $"SslMode={SslMode}" : string.Empty)}";
    }
}