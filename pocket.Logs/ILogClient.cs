using System.Threading.Tasks;

namespace pocket.Logs
{
    public interface ILogClient
    {
        Task<string> GetRawLogs(QueryParameters parameters);

        Task<string> GetRawLogs(string? title = default, string? map = default, string? uploader = default,
            uint limit = 1000, uint offset = 0, params string[] players);

        Task<Query> GetLogs(string? title = default, string? map = default, string? uploader = default,
            uint limit = 1000, uint offset = 0, params string[] players);

        Task<string> GetRawLog(int id);

        Task<LogDto> GetLog(int id);
    }
}