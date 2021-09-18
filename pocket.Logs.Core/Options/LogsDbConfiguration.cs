using System.Text.Json.Serialization;

namespace pocket.Logs.Core.Options
{
    public class LogsDbConfiguration
    {
        public const string LogsDb = "LogsDb";
        
        public string Host { get; set; }

        public string Port { get; set; }

        public string Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public string ConnectionString =>
            $"Host={Host}:{Port};Database={Database};Username={Username};Password={Password}";
    }
}