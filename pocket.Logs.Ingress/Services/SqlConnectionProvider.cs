using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using pocket.Logs.Core.Interfaces;

namespace pocket.Logs.Ingress.Services
{
    public class SqlConnectionProvider : ISqlConnectionProvider
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionProvider(IConfiguration configuration) => _configuration = configuration;

        public string? GetConnectionString(string name = "Default") => _configuration.GetConnectionString(name);

        public SqlConnection GetConnection(string name = "Default")
        {
            var str = GetConnectionString(name);
            if (str == null) throw new ArgumentOutOfRangeException(nameof(name));

            return new SqlConnection(str);
        }
    }
}