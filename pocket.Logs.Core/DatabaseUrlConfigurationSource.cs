using Microsoft.Extensions.Configuration;

namespace pocket.Logs.Core
{
    public class DatabaseUrlConfigurationSource : IConfigurationSource
    {
        private readonly string _databaseUrl;
        private readonly string _path;

        public DatabaseUrlConfigurationSource(string databaseUrl, string path)
        {
            _databaseUrl = databaseUrl;
            _path = path;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new DatabaseUrlConfigurationProvider(_databaseUrl, _path);
    }
}