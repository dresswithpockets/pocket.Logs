namespace pocket.Logs.Ingress
{
    public class LogsTfIngressConfiguration
    {
        public const string LogsTfIngress = "LogsTfIngress";
        
        public uint BulkLimit { get; set; }
        public uint QueryLimit { get; set; }
        public string Cron { get; set; }
    }
}