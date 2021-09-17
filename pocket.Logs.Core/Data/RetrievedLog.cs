using System;

namespace pocket.Logs.Core.Data
{
    public class RetrievedLog
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Map { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime RetrievedAt { get; set; }
        public int Views { get; set; }
        public int Players { get; set; }
        public bool Processed { get; set; }
    }
}