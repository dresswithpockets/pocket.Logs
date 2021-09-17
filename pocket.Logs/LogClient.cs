using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace pocket.Logs
{
    public class LogClient : ILogClient, IDisposable
    {
        private readonly HttpClient _client;

        public LogClient(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://logs.tf/api/");
        }

        public Task<string> GetRawLogs(QueryParameters parameters) => GetRawLogs(parameters.Title, parameters.Map,
            parameters.Uploader, parameters.Limit, parameters.Offset, parameters.Players);

        public async Task<string> GetRawLogs(string? title = default, string? map = default, string? uploader = default,
            uint limit = 1000, uint offset = 0, params string[] players)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (title != null) query["title"] = title;
            if (map != null) query["map"] = map;
            if (uploader != null) query["uploader"] = uploader;
            if (players.Length > 0) query["player"] = string.Join(',', players);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();

            var response = await _client.GetAsync($"v1/log?{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Query> GetLogs(string? title = default, string? map = default, string? uploader = default,
            uint limit = 1000, uint offset = 0, params string[] players)
        {
            var json = await GetRawLogs(title, map, uploader, limit, offset, players);
            var query = JsonSerializer.Deserialize<Query>(json,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return query;
        }

        public async Task<string> GetRawLog(int id) => await _client.GetStringAsync("v1/log/{id}");

        public async Task<LogDto> GetLog(int id)
        {
            var json = await GetRawLog(id);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            return JsonSerializer.Deserialize<LogDto>(json, options);
        }

        public void Dispose()
        {
            _client.Dispose();
            
            GC.SuppressFinalize(this);
        }
    }
}