
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace pocket.Logs.Core
{
    public class DatabaseUrlConfigurationProvider : ConfigurationProvider
    {
        private readonly string _databaseUrl;
        private readonly string _path;

        public DatabaseUrlConfigurationProvider(string databaseUrl, string path)
        {
            _databaseUrl = databaseUrl;
            _path = path;
        }

        public override void Load()
        {
            var uri = new Uri(_databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            var dbname = uri.LocalPath.TrimStart('/');
            var path = string.IsNullOrWhiteSpace(_path) ? string.Empty : $"{_path}:";

            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                [$"{path}Host"] = uri.Host,
                [$"{path}Port"] = uri.Port.ToString(),
                [$"{path}Username"] = userInfo[0],
                [$"{path}Password"] = userInfo[1],
                [$"{path}Database"] = dbname,
            };
        }
    }
}