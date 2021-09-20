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

        public string? CaCert { get; set; }

        [JsonIgnore]
        public string ConnectionString
        {
            get
            {
                var sb = new System.Text.StringBuilder();
                if (SslMode != null) {
                    sb.Append($"SslMode={SslMode};");
                    if (SslMode != Npgsql.SslMode.Disable && CaCert == null)
                        sb.Append($"TrustServerCertificate=True;");
                }
                var ssl = sb.ToString();
                return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};{ssl}Pooling=true;";
            }
        }
    }
}