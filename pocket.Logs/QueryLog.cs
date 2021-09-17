using System;
using System.Text.Json.Serialization;

namespace pocket.Logs
{
    public record Query(bool Success, int Results, int Total, QueryParameters Parameters, QueryLog[] Logs);

    public record QueryParameters([property: JsonPropertyName("player")] string[] Players, string Uploader,
        string Title, string Map, uint Limit, uint Offset);

    public record QueryLog(int Id, string Title, string Map, [property: JsonPropertyName("date")] ulong Timestamp,
        int Views, int Players)
    {
        [JsonIgnore]
        public DateTime Date => DateTime.UtcNow.AddSeconds(Timestamp);
    }
}